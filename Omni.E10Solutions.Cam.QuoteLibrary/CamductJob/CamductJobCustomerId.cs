using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class CamductJobCustomerId
    {
        public readonly string CustomerId;
        public readonly CamductJob Job;

        public CamductJobCustomerId(string customerId, CamductJob job)
        {
            CustomerId = customerId;
            Job = job;
        }
    }
}
