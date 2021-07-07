using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class OrderMapException : Exception
    {
        public OrderMapException(CamductJob job, Exception ex)
            : base("Job " + job.Name + " failed to map to an Order.", ex) { }
    }
}
