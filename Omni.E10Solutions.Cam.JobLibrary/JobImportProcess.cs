using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using log4net;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public static class JobImportProcess
    {
        public static void Run(DirectoryParameter dir, ILog logger, string appServerUrl)
        {
            Epicor10 epicor = null;

            try
            {
                // * grab the files
                var grabFilesService = new GrabFilesService();
                var rawFileData = grabFilesService.Grab(dir); // returns the file data

                if (rawFileData == null) return; // quit if we brought nothing in.

                // * convert the file data into strongly typed and highly structured CamductJobs
                var camJobs = CamductJobCollection.Create(rawFileData);

                // * remove any corrupted jobs
                camJobs.PurgeCorruptedOrInvalidJobs();

                // * if we purged all the jobs, exit now
                if (camJobs.Count == 0)
                {
                    // * process bad jobs
                    var invalidationService = new InvalidateJobFilesService(dir);
                    invalidationService.InvalidateJobs(camJobs.GetPurgedJobs());
                    return;
                }

                // * connect to epicor
                var epi = new EpicorParameter(appServerUrl, camJobs.Company, camJobs.Plant);
                epicor = new Epicor10(epi);
                var epicorCache = new Epicor10Cache(epicor);

                // * cache epicor data.
                var cacheService = new CacheService(epicorCache);
                cacheService.Cache(camJobs);

                // * remove any jobs that are corrupted or invalid.
                camJobs.PurgeCorruptedOrInvalidJobs();

                // * create EpicorQuotes for each CamductJob.
                var camToEpicorJobsService = new ConvertJobsToJobEntriesService(epicor, epicorCache);
                var jobEntries = camToEpicorJobsService.Convert(camJobs);

                // * commit the quotes
                var persistJobEntryService = new PersistJobEntryService(epicor, logger);
                foreach (var jobEntry in jobEntries)
                {
                    persistJobEntryService.PersistJobEntry(jobEntry);
                }
                camJobs.PurgeCorruptedOrInvalidJobs();

                // * process bad jobs
                var invalidateBadJobsService = new InvalidateJobFilesService(dir);
                invalidateBadJobsService.InvalidateJobs(camJobs.GetPurgedJobs());

                // * archive committed jobs
                var fileService = new MoveFilesService(dir);
                fileService.Archive(camJobs.GetFilePaths());
            }
            catch (Exception ex)
            {
                var msg = "An unexpected error of type " + ex.GetType() + " occurred. The process is still running, but the import failed. See the log for more details.";
                logger.Error(msg, ex);

                // * send an email with information regarding the error.
            }
            finally
            {
                if (epicor != null)
                    epicor.CloseSession();
            }
        }
    }
}
