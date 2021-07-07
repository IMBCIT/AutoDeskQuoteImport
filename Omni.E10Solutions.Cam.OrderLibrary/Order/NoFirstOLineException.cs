using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class NoFirstOLineException : Exception
    {
        public NoFirstOLineException(string jobName)
            : base("There was no line found in the O file of the job " + jobName + ".")
        {

        }
    }
}
