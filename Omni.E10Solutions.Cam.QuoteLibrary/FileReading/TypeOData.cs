using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeOData
    {
        public readonly string CustomerId;
        public readonly string ProjectName;
        public readonly string JobFileName;
        public readonly string Date;
        public readonly string Time;
        public readonly string EpicorGroup;
        public readonly string ItemNo;
        public readonly string Qty;
        public readonly string ItemTag;
        public readonly string ItemSpool;
        public readonly string ItemAlias;
        public readonly string DoubleWall;
        public readonly string CID;
        public readonly string ItemDescription;
        public readonly string SizeEnd1;
        public readonly string SizeEnd2;
        public readonly string SizeEnd3;
        public readonly string SizeEnd4;
        public readonly string ItemLengthOrAngle;
        public readonly string Material;
        public readonly string WireGauge;
        public readonly string BaseWeight;
        public readonly string InsulationMaterial;
        public readonly string InsulationThickness;
        public readonly string InsulationArea;
        public readonly string DWSkinMaterial;
        public readonly string DWSkinGauge;
        public readonly string DWSkinWeight;
        public readonly string WELD_Time_mins;
        public readonly string PRFB_Time_mins;
        public readonly string BURN_Time_mins;
        public readonly string COIL_Time_mins;
        public readonly string RECT_Time_mins;
        public readonly string ROND_Time_mins;
        public readonly string SPIR_Time_mins;
        public readonly string NonGalv;
        public readonly string CompanyEmail;
        public readonly string NumberCutSheets;
        public readonly string DrawingNumber;
        public readonly string SpecSection;
        public readonly string Addendums;
        public readonly string MAJOtherFileName;
        public readonly string SentVia;

        public TypeOData(string[] data)
        {
            if (data.Length < 36)
                throw new NotEnoughColumnsException("O");

            CustomerId = data[0].StripQuotationMarks().ToUpper();
            ProjectName = data[1].StripQuotationMarks();
            JobFileName = data[2].StripQuotationMarks();
            Date = data[3].StripQuotationMarks();
            Time = data[4].StripQuotationMarks();
            EpicorGroup = data[5].StripQuotationMarks();
            ItemNo = data[6].StripQuotationMarks();
            Qty = data[7].StripQuotationMarks();
            ItemTag = data[8].StripQuotationMarks();
            ItemSpool = data[9].StripQuotationMarks();
            ItemAlias = data[10].StripQuotationMarks();
            DoubleWall = data[11].StripQuotationMarks();
            CID = data[12].StripQuotationMarks();
            ItemDescription = data[13].StripQuotationMarks();
            SizeEnd1 = data[14].StripQuotationMarks();
            SizeEnd2 = data[15].StripQuotationMarks();
            SizeEnd3 = data[16].StripQuotationMarks();
            SizeEnd4 = data[17].StripQuotationMarks();
            ItemLengthOrAngle = data[18].StripQuotationMarks();
            Material = data[19].StripQuotationMarks();
            WireGauge = data[20].StripQuotationMarks();
            BaseWeight = data[21].StripQuotationMarks();
            InsulationMaterial = data[22].StripQuotationMarks();
            InsulationThickness = data[23].StripQuotationMarks();
            InsulationArea = data[24].StripQuotationMarks();
            DWSkinMaterial = data[25].StripQuotationMarks();
            DWSkinGauge = data[26].StripQuotationMarks();
            DWSkinWeight = data[27].StripQuotationMarks();
            WELD_Time_mins = data[28].StripQuotationMarks();
            PRFB_Time_mins = data[29].StripQuotationMarks();
            BURN_Time_mins = data[30].StripQuotationMarks();
            COIL_Time_mins = data[31].StripQuotationMarks();
            RECT_Time_mins = data[32].StripQuotationMarks();
            ROND_Time_mins = data[33].StripQuotationMarks();
            SPIR_Time_mins = data[34].StripQuotationMarks();
            NonGalv = data[35].StripQuotationMarks();
            if (data.Length > 36)
            {
                CompanyEmail = data[36].StripQuotationMarks();
                if (data.Length > 37)
                {
                    NumberCutSheets = data[37].StripQuotationMarks();
                    DrawingNumber = data[38].StripQuotationMarks();
                    SpecSection = data[39].StripQuotationMarks();
                    Addendums = data[40].StripQuotationMarks();
                    MAJOtherFileName = data[41].StripQuotationMarks();
                    SentVia = data[42].StripQuotationMarks();
                }
            }
        }
    }
}
