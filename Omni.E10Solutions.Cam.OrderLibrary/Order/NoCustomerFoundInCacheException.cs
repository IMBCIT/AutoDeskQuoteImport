using System;
using System.Runtime.Serialization;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    [Serializable]
    internal class NoCustomerFoundInCacheException : Exception
    {
        public NoCustomerFoundInCacheException(string customerId)
            : base("Customer " + customerId + " not found in the cache.")
        {

        }
    }
}