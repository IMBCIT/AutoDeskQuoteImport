using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeAData
    {
        public readonly string JobName;
        public readonly string Date;
        public readonly string Time;
        public readonly string ItemNo;
        public readonly string AncillaryName;
        public readonly string AncillaryPartNo;
        public readonly string AncillaryLength;
        public readonly string AncillaryQty;

        public TypeAData(string[] data)
        {
            if (data.Length < 8)
                throw new NotEnoughColumnsException("A");

            JobName = data[0].StripQuotationMarks();
            Date = data[1].StripQuotationMarks();
            Time = data[2].StripQuotationMarks();
            ItemNo = data[3].StripQuotationMarks();
            AncillaryName = data[4].StripQuotationMarks();
            AncillaryPartNo = data[5].StripQuotationMarks();
            AncillaryLength = data[6].StripQuotationMarks();
            AncillaryQty = data[7].StripQuotationMarks();
        }
    }
}
