using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class JobMapException : Exception
    {
        public JobMapException(CamductJob job, Exception ex)
            : base("Job " + job.Name + " failed to map to a Quote.", ex) { }
    }
}
