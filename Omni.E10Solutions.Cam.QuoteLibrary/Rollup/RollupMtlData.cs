using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class RollupMtlData
    {
        public readonly string PartNumber;
        public readonly decimal QtyPer;
        public readonly string IUM;
        public readonly string EstScrapType;
        public readonly decimal EstScrap;

        public RollupMtlData(string partNumber, decimal qtyPer, string ium)
        {
            PartNumber = partNumber;
            QtyPer = qtyPer;
            IUM = ium;
            EstScrapType = string.Empty;
            EstScrap = 0m;
        }

        public RollupMtlData(string partNumber, decimal qtyPer, string ium, decimal estScrap)
        {
            PartNumber = partNumber;
            QtyPer = qtyPer;
            IUM = ium;
            EstScrapType = "%";
            EstScrap = estScrap;
        }
    }
}
