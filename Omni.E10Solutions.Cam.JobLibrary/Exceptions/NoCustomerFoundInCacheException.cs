using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class NoCustomerFoundInCacheException : Exception
    {
        public NoCustomerFoundInCacheException(string customerId)
            : base("Customer " + customerId + " not found in the cache.")
        {

        }
    }
}
