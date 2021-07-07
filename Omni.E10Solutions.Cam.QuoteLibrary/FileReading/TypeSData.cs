using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeSData
    {
        public readonly string JobName;
        public readonly string Date;
        public readonly string Time;
        public readonly string LinearThickness;
        public readonly string Material;
        public readonly string Gauge;
        public readonly string BlankWeight;
        public readonly string Insulation;
        public readonly string CutArea;
        public readonly string ScrapWt;
        public readonly string ScrapSF;

        public TypeSData(string[] data)
        {
            if (data.Length < 11)
                throw new NotEnoughColumnsException("S");

            JobName = data[0].StripQuotationMarks();
            Date = data[1].StripQuotationMarks();
            Time = data[2].StripQuotationMarks();
            Insulation = data[3].StripQuotationMarks();
            Material = data[4].StripQuotationMarks();
            Gauge = data[5].StripQuotationMarks();
            LinearThickness = data[6].StripQuotationMarks();
            BlankWeight = data[7].StripQuotationMarks();
            CutArea = data[8].StripQuotationMarks();
            ScrapWt = data[9].StripQuotationMarks();
            ScrapSF = data[10].StripQuotationMarks();
        }
    }
}
