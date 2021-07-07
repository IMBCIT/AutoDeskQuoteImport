using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class CamductJobCollection : List<CamductJob>
    {
        private List<CamductJob> _corruptedJobs = new List<CamductJob>();
        private List<CamductJob> _invalidJobs = new List<CamductJob>();

        public static CamductJobCollection Create(RawFileDataDictionary rawFileData)
        {
            var rawJobData = RawJobDataDictionary.Create(rawFileData);
            var jobs = new CamductJobCollection();
            foreach (var jobData in rawJobData)
            {
                var job = rawJobData.BuildCamductJob(jobData.Key);
                jobs.Add(job);
            }

            return jobs;
        }

        public CamductJobCollection() : base() { }
        public CamductJobCollection(IEnumerable<CamductJob> collection) : base(collection) { }

        public IEnumerable<string> GetDistinctCustomerIds()
        {
            return this.Select(j => j.GetCustomerId()).Distinct();
        }

        public IEnumerable<string> GetDistinctNonDtlPartNumbers()
        {
            return this.SelectMany(j => j.GetNonDtlPartNumbers()).Distinct();
        }

        public IEnumerable<string> GetDistinctDtlPartNumbers()
        {
            return this.SelectMany(j => j.GetDtlOnlyPartNumbers()).Distinct();
        }

        public IEnumerable<string> GetAllDistinctPartNumbers()
        {
            return this.SelectMany(j => j.GetAllPartNumbers()).Distinct();
        }

        public void RegisterCustomerError(CustomerError error)
        {
            foreach (var job in this)
            {
                if (job.GetCustomerId() == error.CustomerId)
                    job.RegisterException(error.Exception);
            }
        }

        public void RegisterPartError(PartError error)
        {
            foreach (var job in this)
            {
                if (job.GetNonDtlPartNumbers().Contains(error.PartNumber))
                    job.RegisterException(error.Exception);
            }
        }

        public void RegisterPartCostError(PartCostError error)
        {
            foreach (var job in this)
            {
                if (job.GetNonDtlPartNumbers().Contains(error.PartNumber))
                    job.RegisterException(error.Exception);
            }
        }

        public CamductJobCollection GetPurgedJobs()
        {
            PurgeCorruptedOrInvalidJobs();
            var allPurgedJobs = _corruptedJobs.Concat(_invalidJobs).Distinct();
            return new CamductJobCollection(allPurgedJobs);
        }

        public CamductJobCollection PurgeCorruptedOrInvalidJobs()
        {
            // get the corrupted or invalid jobs
            var corruptedJobs = this.Where(j => j.IsCorrupted()).ToList();
            var invalidJobs = this.Where(j => j.IsInvalid()).ToList();

            // add the corrupted and invalid jobs to the private lists
            _corruptedJobs.AddRange(corruptedJobs);
            _invalidJobs.AddRange(invalidJobs);

            // remove them from the collection
            this.RemoveAll(j => j.IsCorrupted() || j.IsInvalid());

            // return purged
            var purgedJobs = corruptedJobs.Concat(invalidJobs).Distinct();
            return new CamductJobCollection(purgedJobs);
        }

        public IEnumerable<string> GetFilePaths()
        {
            return this.SelectMany(j => j.GetFilePaths());
        }

        public string Plant => this.FirstOrDefault()?.Plant;
        public string Company => this.FirstOrDefault()?.Company;
    }
}
