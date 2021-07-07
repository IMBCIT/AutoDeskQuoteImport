using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class ConvertJobsToJobEntriesService
    {
        Epicor10 _epicor;
        Epicor10Cache _cache;

        public ConvertJobsToJobEntriesService(Epicor10 epicor, Epicor10Cache cache)
        {
            _epicor = epicor;
            _cache = cache;
        }

        public List<IJobEntry> Convert(CamductJobCollection camJobs)
        {
            var udService = new UdService(_epicor);
            var jobEntryFactory = new JobEntryFactory(_cache, udService);
            var jobEntries = new List<IJobEntry>();
            foreach (var camJob in camJobs)
            {
                try
                {
                    var jobs = jobEntryFactory.CreateJobEntries(camJob);
                    jobEntries.AddRange(jobs);
                    //var jobEntry = jobEntryFactory.CreateJobEntry(camJob);
                    //jobEntries.Add(jobEntry);
                }
                catch (Exception ex)
                {
                    var jobEntryMapException = new JobMapException(camJob, ex);
                    camJob.RegisterException(jobEntryMapException);
                }
            }

            return jobEntries;
        }
    }
}
