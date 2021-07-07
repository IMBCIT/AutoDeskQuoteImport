using System;
using System.Runtime.Serialization;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    internal class NoPartFoundInCacheException : Exception
    {
        public NoPartFoundInCacheException(string partNumber)
            : base("Part " + partNumber + " not found in the cache.")
        {

        }
    }
}