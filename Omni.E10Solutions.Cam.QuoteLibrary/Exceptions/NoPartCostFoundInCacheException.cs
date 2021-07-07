using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class NoPartCostFoundInCacheException : Exception
    {
        public NoPartCostFoundInCacheException(string partNumber)
            : base("Part '" + partNumber + "' not found in the cache.")
        {

        }
    }
}
