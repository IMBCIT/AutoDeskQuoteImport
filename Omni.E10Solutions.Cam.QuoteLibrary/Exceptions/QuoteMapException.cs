using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class QuoteMapException : Exception
    {
        public QuoteMapException(CamductJob job, Exception ex) 
            : base("Job " + job.Name + " failed to map to a Quote.", ex) { }
    }
}
