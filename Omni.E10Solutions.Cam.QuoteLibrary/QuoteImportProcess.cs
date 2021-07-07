using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public static class QuoteImportProcess
    {
        public static void Run(DirectoryParameter dir, ILog logger, string appServerUrl)
        {
            var mode = CamConversionMode.LineItem;
            EpicorServiceFactory epicorServices = null;
            try
            {

                // initialize file services
                var fileServices = new FileServices(dir);

                // Phase 1 - Grab files, Move files, Validate files 
                // --> output : CamductJobCollection
                // --> states : clean exit or files in error dir and exit or files in in process and continue execution
                fileServices.ImitateUserLogin();

                var jobs = GrabCamJobs(fileServices);
                fileServices.ImitateUserLogout();
                if (jobs == null) return; // quit if we brought nothing in.

                // initialize epicor services
                var epi = new EpicorParameter(appServerUrl, jobs.Company, jobs.Plant);
                epicorServices = new EpicorServiceFactory(epi, logger, mode);


                // connect epicor services
                epicorServices.Connect();


                // Phase 2 - Hydrate jobs with epicor data, perfrom mapping logic
                // --> output : List<IQuote> 
                // --> states : 
                var quotes = ConvertCamJobsToQuotes(epicorServices, jobs);

                // Phase 3 - Import the Quotes to Epicor
                // --> output : log text to log source
                // --> state : files in in process, files in archive new Quotes created, files in error with exception file.

                CommitQuote(epicorServices, fileServices, jobs, quotes);
            }
            catch (Exception ex)
            {
                var msg = "An unexpected error of type " + ex.GetType() + " occurred. The process is still running, but the import failed. Associated files are still located in the InProcessing directory.";
                logger.Error(msg, ex);

                // * send an email with information regarding the error.
            }
            finally
            {
                epicorServices?.Disconnect();
            }
        }

        static CamductJobCollection GrabCamJobs(FileServices fileServices)
        {
            // *grab the file paths
            var filePaths = fileServices.GetFilePaths();
            if (filePaths.Count() == 0)
            {
                return null; // quit if no files in the dir
            }

            // * move the files to the in process dir 
            filePaths = fileServices.Process(filePaths);

            // * read the file data into memory
            var rawFileData = fileServices.ReadFiles(filePaths);
            if (rawFileData == null)
            {
                return null; // quit if files are empty
            }

            // * convert the file data into strongly typed and highly structured CamductJobs
            var jobs = CamductJobCollection.Create(rawFileData);

            // * remove any corrupted jobs
            jobs.PurgeCorruptedOrInvalidJobs();

            // * if we purged all the jobs, exit now
            if (jobs.Count == 0)
            {
                // * process bad jobs
                fileServices.InvalidateJobs(jobs.GetPurgedJobs());
                return null;
            }

            return jobs;
        }

        static List<IQuote> ConvertCamJobsToQuotes(EpicorServiceFactory epicorServices, CamductJobCollection jobs)
        {
            // * cache epicor data.
            var cacheService = epicorServices.CreateCacheService();
            cacheService.Cache(jobs);

            // * remove any jobs that are corrupted or invalid.
            jobs.PurgeCorruptedOrInvalidJobs();

            // * create EpicorQuotes for each CamductJob.
            var jobsToQuotesService = epicorServices.CreateConvertService();
            var quotes = jobsToQuotesService.Convert(jobs);

            for (int i = 0; i < quotes.Count; i++)
            {
                var quote = quotes[i];

                foreach (Erp.BO.UpdExtQuoteDataSet.QuoteDtlRow qDtl in quote.GetQuoteDataSet().QuoteDtl.Rows)
                {
                    var p = cacheService._cache.DtlPartCache.FirstOrDefault(x => x.PartNum.Equals(qDtl.PartNum));

                    if (p != null)
                    {
                        qDtl.SellingExpectedUM = p.IUM;
                    }

                    //if(cacheService.Cache)
                }
            }

            return quotes;
        }

        static void CommitQuote(EpicorServiceFactory epicorServices, FileServices fileServices, CamductJobCollection jobs, List<IQuote> quotes)
        {
            // * commit the quotes
            var persistQuoteService = epicorServices.CreatePersistQuoteService();
            foreach (var quote in quotes)
            {
                persistQuoteService.PersistQuote(quote);
            }
            jobs.PurgeCorruptedOrInvalidJobs();

            fileServices.ImitateUserLogin();
            // * process bad jobs
            fileServices.InvalidateJobs(jobs.GetPurgedJobs());

            // * archive committed jobs
            fileServices.Archive(jobs.GetFilePaths());
            fileServices.ImitateUserLogout();
        }
    }
}
