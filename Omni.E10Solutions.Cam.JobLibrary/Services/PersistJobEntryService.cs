using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class PersistJobEntryService
    {
        Epicor10 _store;
        ILog _logger;

        public PersistJobEntryService(Epicor10 store, ILog logger)
        {
            _store = store;
            _logger = logger;
        }

        public bool PersistJobEntry(IJobEntry job)
        {
            // return PersistJobEntry_Slow(job);

            // assume if this fails, no UD.
            if (PersistJobEntry_Slow(job))
            {
                // assume if UD fails... just log and move on.
                var error = PersistReportingFields(job);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    _logger.Error(error);
                }

                return true;
            }
            return false;
        }

        public bool PersistJobEntry_Orig(IJobEntry job)
        {
            var nextJobNum = _store.GetNextJobNum();
            job.SetJobNum(nextJobNum);
            var jobCommitResult = _store.CommitJob(job.GetJobEntryDataSet());
            var jobNum = job.GetJobNum();

            // read the result message
            var isCommitted = ReadCommitResultMessage("Job", jobCommitResult, jobNum);

            if (!isCommitted)
            {
                job.Job.RegisterException(new Exception(jobCommitResult));
                return false;
            }

            return true;
        }

        public bool PersistJobEntry_Slow(IJobEntry job)
        {
            var nextJobNum = _store.GetNextJobNum();
            job.SetJobNum(nextJobNum);
            var jobCommitResult = _store.CommitJob_Long(job.GetJobEntryDataSet());
            var jobNum = job.GetJobNum();

            // read the result message
            var isCommitted = ReadCommitResultMessage("Job", jobCommitResult, jobNum);

            if (!isCommitted)
            {
                job.Job.RegisterException(new Exception(jobCommitResult));
                return false;
            }

            return true;
        }

        bool ReadCommitResultMessage(string dataSetName, string resultMessage, string jobNum)
        {
            if (string.IsNullOrWhiteSpace(resultMessage))
            {
                _logger.Info("Commited " + dataSetName + ". Status: Successful! JobNum: " + jobNum);
                return true;
            }
            else
            {
                _logger.Info("Commited " + dataSetName + ". Status: " + resultMessage);
                if (string.IsNullOrWhiteSpace(jobNum))
                {
                    _store.DeleteJob(jobNum);
                }
                return false;
            }
        }

        string PersistReportingFields(IJobEntry jobEntry)
        {
            var commitResult = _store.CommitReportingData(jobEntry.GetReportingDataSet());

            var isCommitted = ReadCommitResultMessage("Report Fields (UD03)", commitResult, jobEntry.GetJobNum());

            if (!isCommitted)
            {
                return commitResult;
            }

            return null;
        }
    }
}
