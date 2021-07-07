using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class NoPartFoundInCacheException : Exception
    {
        public NoPartFoundInCacheException(string partNumber)
            : base("Part " + partNumber + " not found in the cache.")
        {

        }
    }
}
