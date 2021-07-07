using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class PartCostError
    {
        public readonly string PartNumber;
        public readonly string CostId;
        public readonly Exception Exception;

        public PartCostError(string partNumber, string costId)
        {
            PartNumber = partNumber;
            CostId = costId;
            Exception = new Exception("Part Number " + partNumber + " does not have cost information for cost id " + costId + ".");
        }
    }
}
