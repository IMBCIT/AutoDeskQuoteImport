using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class CustomerError
    {
        public readonly string CustomerId;
        public readonly Exception Exception;

        public CustomerError(string customerId, Exception ex)
        {
            CustomerId = customerId;
            Exception = ex;
        }

        public CustomerError(Tuple<string, Exception> stringExceptionTuple)
        {
            CustomerId = stringExceptionTuple.Item1;
            Exception = stringExceptionTuple.Item2;
        }
    }
}
