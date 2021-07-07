using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    public static class OrderImportProcess
    {
        public static void Run(DirectoryParameter dir, string plant, ILog logger, string appServerUrl)
        {
            Epicor10 epicor = null;

            try
            {
                // * grab the files
                var grabFilesService = new GrabFilesService();
                var rawFileData = grabFilesService.Grab(dir); // returns the file data

                if (rawFileData == null) return; // quit if we brought nothing in.

                // * convert the file data into strongly typed and highly structured CamductJobs
                var jobs = CamductJobCollection.Create(rawFileData, plant);

                // * remove any corrupted jobs
                jobs.PurgeCorruptedOrInvalidJobs();

                // * if we purged all the jobs, exit now
                if (jobs.Count == 0)
                {
                    // * process bad jobs
                    var invalidationService = new InvalidateJobFilesService(dir);
                    invalidationService.InvalidateJobs(jobs.GetPurgedJobs());
                    return;
                }

                // * connect to epicor
                epicor = new Epicor10(appServerUrl);
                var epicorCache = new Epicor10Cache(epicor);

                // * cache epicor data.
                var cacheService = new CacheService(epicorCache);
                cacheService.Cache(jobs);

                // * remove any jobs that are corrupted or invalid.
                jobs.PurgeCorruptedOrInvalidJobs();

                // * create EpicorQuotes for each CamductJob.
                var conversionService = new ConvertJobsToOrdersService(epicor, epicorCache);
                var orders = conversionService.Convert(jobs);

                // * commit the orders
                var persistQuoteService = new PersistOrderService(epicor, logger);
                foreach (var order in orders)
                {
                    persistQuoteService.PersistOrder(order);
                }
                jobs.PurgeCorruptedOrInvalidJobs();

                // * process bad jobs
                var invalidateBadJobsService = new InvalidateJobFilesService(dir);
                invalidateBadJobsService.InvalidateJobs(jobs.GetPurgedJobs());

                // * archive committed jobs
                var fileService = new MoveFilesService(dir);
                fileService.Archive(jobs.GetFilePaths());
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
