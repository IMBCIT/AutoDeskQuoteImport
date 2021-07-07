using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class TypeOData
    {
        List<IField> _fields = new List<IField>();

        public TypeOData(string[] data)
        {
            if (data.Length < 45)
                throw new NotEnoughColumnsException("O");

            SetCustomerIdField(0, data[0]);
            SetProjectNameField(1, data[1]);
            SetJobFileNameField(2, data[2]);
            SetDateField(3, data[3]);
            SetTimeField(4, data[4]);
            SetEpicorGroupField(5, data[5]);
            SetItemNoField(6, data[6]);
            SetQtyField(7, data[7]);
            SetItemTagField(8, data[8]);
            SetItemSpoolField(9, data[9]);
            SetItemAliasField(10, data[10]);
            SetDoubleWallField(11, data[11]);
            SetCIDField(12, data[12]);
            SetItemDescriptionField(13, data[13]);
            SetSizeEnd1Field(14, data[14]);
            SetSizeEnd2Field(15, data[15]);
            SetSizeEnd3Field(16, data[16]);
            SetSizeEnd4Field(17, data[17]);
            SetItemLengthOrAngleField(18, data[18]);
            SetMaterialField(19, data[19]);
            SetWireGaugeField(20, data[20]);
            SetBaseWeightField(21, data[21]);
            SetInsulationMaterialField(22, data[22]);
            SetInsulationThicknessField(23, data[23]);
            SetInsulationAreaField(24, data[24]);
            SetDWSkinMaterialField(25, data[25]);
            SetDWSkinGaugeField(26, data[26]);
            SetDWSkinWeightField(27, data[27]);
            SetWELD_Time_minsField(28, data[28]);
            SetPRFB_Time_minsField(29, data[29]);
            SetBURN_Time_minsField(30, data[30]);
            SetCOIL_Time_minsField(31, data[31]);
            SetRECT_Time_minsField(32, data[32]);
            SetROND_Time_minsField(33, data[33]);
            SetSPIR_Time_minsField(34, data[34]);
            SetNonGalvField(35, data[35]);
            SetCompanyEmailField(36, data[36]);
            SetNumberCutSheetsField(37, data[37]);
            SetDrawingNumberField(38, data[38]);
            SetSpecSectionField(39, data[39]);
            SetAddendumsField(40, data[40]);
            SetMAJOtherFileNameField(41, data[41]);
            SetSentViaField(42, data[42]);
            SetPressureClassField(43, data[43]);
            SetPSST_Time_minsField(44, data[44]);
            SetCompanyField(45, data[45]);
            SetPlantField(46, data[46]);

            _fields.Add(CustomerIdField);
            _fields.Add(ProjectNameField);
            _fields.Add(JobFileNameField);
            _fields.Add(DateField);
            _fields.Add(TimeField);
            _fields.Add(EpicorGroupField);
            _fields.Add(ItemNoField);
            _fields.Add(QtyField);
            _fields.Add(ItemTagField);
            _fields.Add(ItemSpoolField);
            _fields.Add(ItemAliasField);
            _fields.Add(DoubleWallField);
            _fields.Add(CIDField);
            _fields.Add(ItemDescriptionField);
            _fields.Add(SizeEnd1Field);
            _fields.Add(SizeEnd2Field);
            _fields.Add(SizeEnd3Field);
            _fields.Add(SizeEnd4Field);
            _fields.Add(ItemLengthOrAngleField);
            _fields.Add(MaterialField);
            _fields.Add(WireGaugeField);
            _fields.Add(BaseWeightField);
            _fields.Add(InsulationMaterialField);
            _fields.Add(InsulationThicknessField);
            _fields.Add(InsulationAreaField);
            _fields.Add(DWSkinMaterialField);
            _fields.Add(DWSkinGaugeField);
            _fields.Add(DWSkinWeightField);
            _fields.Add(WELD_Time_minsField);
            _fields.Add(PRFB_Time_minsField);
            _fields.Add(BURN_Time_minsField);
            _fields.Add(COIL_Time_minsField);
            _fields.Add(RECT_Time_minsField);
            _fields.Add(ROND_Time_minsField);
            _fields.Add(SPIR_Time_minsField);
            _fields.Add(NonGalvField);
            _fields.Add(CompanyEmailField);
            _fields.Add(NumberCutSheetsField);
            _fields.Add(DrawingNumberField);
            _fields.Add(SpecSectionField);
            _fields.Add(AddendumsField);
            _fields.Add(MAJOtherFileNameField);
            _fields.Add(SentViaField);
            _fields.Add(PressureClassField);
            _fields.Add(PSTT_Time_minsField);
            _fields.Add(CompanyField);
            _fields.Add(PlantField);
        }

        public Field<string> CustomerIdField { get; protected set; }
        public Field<string> ProjectNameField { get; protected set; }
        public Field<string> JobFileNameField { get; protected set; }
        public Field<string> DateField { get; protected set; }
        public Field<string> TimeField { get; protected set; }
        public Field<string> EpicorGroupField { get; protected set; }
        public Field<int> ItemNoField { get; protected set; }
        public Field<decimal> QtyField { get; protected set; }
        public Field<string> ItemTagField { get; protected set; }
        public Field<string> ItemSpoolField { get; protected set; }
        public Field<string> ItemAliasField { get; protected set; }
        public Field<string> DoubleWallField { get; protected set; }
        public Field<string> CIDField { get; protected set; }
        public Field<string> ItemDescriptionField { get; protected set; }
        public Field<string> SizeEnd1Field { get; protected set; }
        public Field<string> SizeEnd2Field { get; protected set; }
        public Field<string> SizeEnd3Field { get; protected set; }
        public Field<string> SizeEnd4Field { get; protected set; }
        public Field<decimal?> ItemLengthOrAngleField { get; protected set; }
        public Field<string> MaterialField { get; protected set; }
        public Field<decimal?> WireGaugeField { get; protected set; }
        public Field<decimal?> BaseWeightField { get; protected set; }
        public Field<string> InsulationMaterialField { get; protected set; }
        public Field<decimal?> InsulationThicknessField { get; protected set; }
        public Field<decimal?> InsulationAreaField { get; protected set; }
        public Field<string> DWSkinMaterialField { get; protected set; }
        public Field<string> DWSkinGaugeField { get; protected set; }
        public Field<decimal?> DWSkinWeightField { get; protected set; }
        public Field<decimal?> WELD_Time_minsField { get; protected set; }
        public Field<decimal?> PRFB_Time_minsField { get; protected set; }
        public Field<decimal?> BURN_Time_minsField { get; protected set; }
        public Field<decimal?> COIL_Time_minsField { get; protected set; }
        public Field<decimal?> RECT_Time_minsField { get; protected set; }
        public Field<decimal?> ROND_Time_minsField { get; protected set; }
        public Field<decimal?> SPIR_Time_minsField { get; protected set; }
        public Field<string> NonGalvField { get; protected set; }
        public Field<string> CompanyEmailField { get; protected set; }
        public Field<string> NumberCutSheetsField { get; protected set; }
        public Field<string> DrawingNumberField { get; protected set; }
        public Field<string> SpecSectionField { get; protected set; }
        public Field<string> AddendumsField { get; protected set; }
        public Field<string> MAJOtherFileNameField { get; protected set; }
        public Field<string> SentViaField { get; protected set; }
        public Field<string> PressureClassField { get; protected set; }
        public Field<decimal?> PSTT_Time_minsField { get; protected set; }
        public Field<string> CompanyField { get; protected set; }
        public Field<string> PlantField { get; protected set; }

        void SetCustomerIdField(int position, string value)
        {
            var name = nameof(CustomerIdField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            CustomerIdField = FieldFactory.NewMandatoryField(position, name, formattedValue);
        }

        void SetProjectNameField(int position, string value)
        {
            var name = nameof(ProjectNameField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ProjectNameField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetJobFileNameField(int position, string value)
        {
            var name = nameof(JobFileNameField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            JobFileNameField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDateField(int position, string value)
        {
            var name = nameof(DateField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DateField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetTimeField(int position, string value)
        {
            var name = nameof(TimeField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            TimeField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetEpicorGroupField(int position, string value)
        {
            var name = nameof(EpicorGroupField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            EpicorGroupField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetItemNoField(int position, string value)
        {
            var name = nameof(ItemNoField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            ItemNoField = FieldFactory.NewIntField(position, name, formattedValue);
        }

        void SetQtyField(int position, string value)
        {
            var name = nameof(QtyField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            QtyField = FieldFactory.NewDecimalField(position, name, formattedValue);
        }

        void SetItemTagField(int position, string value)
        {
            var name = nameof(ItemTagField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemTagField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetItemSpoolField(int position, string value)
        {
            var name = nameof(ItemSpoolField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemSpoolField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetItemAliasField(int position, string value)
        {
            var name = nameof(ItemAliasField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemAliasField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDoubleWallField(int position, string value)
        {
            var name = nameof(DoubleWallField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DoubleWallField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetCIDField(int position, string value)
        {
            var name = nameof(CIDField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            CIDField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetItemDescriptionField(int position, string value)
        {
            var name = nameof(ItemDescriptionField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemDescriptionField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSizeEnd1Field(int position, string value)
        {
            var name = nameof(SizeEnd1Field).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SizeEnd1Field = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSizeEnd2Field(int position, string value)
        {
            var name = nameof(SizeEnd2Field).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SizeEnd2Field = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSizeEnd3Field(int position, string value)
        {
            var name = nameof(SizeEnd3Field).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SizeEnd3Field = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSizeEnd4Field(int position, string value)
        {
            var name = nameof(SizeEnd4Field).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SizeEnd4Field = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetItemLengthOrAngleField(int position, string value)
        {
            var name = nameof(ItemLengthOrAngleField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemLengthOrAngleField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetMaterialField(int position, string value)
        {
            var name = nameof(MaterialField);
            var formattedValue = value.StripQuotationMarks();
            MaterialField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetWireGaugeField(int position, string value)
        {
            var name = nameof(WireGaugeField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            WireGaugeField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetBaseWeightField(int position, string value)
        {
            var name = nameof(BaseWeightField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            BaseWeightField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetInsulationMaterialField(int position, string value)
        {
            var name = nameof(InsulationMaterialField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            InsulationMaterialField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetInsulationThicknessField(int position, string value)
        {
            var name = nameof(InsulationThicknessField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            InsulationThicknessField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetInsulationAreaField(int position, string value)
        {
            var name = nameof(InsulationAreaField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            InsulationAreaField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetDWSkinMaterialField(int position, string value)
        {
            var name = nameof(DWSkinMaterialField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DWSkinMaterialField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDWSkinGaugeField(int position, string value)
        {
            var name = nameof(DWSkinGaugeField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DWSkinGaugeField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDWSkinWeightField(int position, string value)
        {
            var name = nameof(DWSkinWeightField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DWSkinWeightField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetWELD_Time_minsField(int position, string value)
        {
            var name = nameof(WELD_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            WELD_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetPRFB_Time_minsField(int position, string value)
        {
            var name = nameof(PRFB_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            PRFB_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetBURN_Time_minsField(int position, string value)
        {
            var name = nameof(BURN_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            BURN_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetCOIL_Time_minsField(int position, string value)
        {
            var name = nameof(COIL_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            COIL_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetRECT_Time_minsField(int position, string value)
        {
            var name = nameof(RECT_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            RECT_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetROND_Time_minsField(int position, string value)
        {
            var name = nameof(ROND_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ROND_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetSPIR_Time_minsField(int position, string value)
        {
            var name = nameof(SPIR_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SPIR_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetNonGalvField(int position, string value)
        {
            var name = nameof(NonGalvField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            NonGalvField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetCompanyEmailField(int position, string value)
        {
            var name = nameof(CompanyEmailField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            CompanyEmailField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetNumberCutSheetsField(int position, string value)
        {
            var name = nameof(NumberCutSheetsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            NumberCutSheetsField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDrawingNumberField(int position, string value)
        {
            var name = nameof(DrawingNumberField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            DrawingNumberField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSpecSectionField(int position, string value)
        {
            var name = nameof(SpecSectionField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SpecSectionField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetAddendumsField(int position, string value)
        {
            var name = nameof(AddendumsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            AddendumsField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetMAJOtherFileNameField(int position, string value)
        {
            var name = nameof(MAJOtherFileNameField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            MAJOtherFileNameField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetSentViaField(int position, string value)
        {
            var name = nameof(SentViaField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            SentViaField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetPressureClassField(int position, string value)
        {
            var name = nameof(PressureClassField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            PressureClassField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetPSST_Time_minsField(int position, string value)
        {
            var name = nameof(PSTT_Time_minsField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            PSTT_Time_minsField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        private void SetCompanyField(int position, string value)
        {
            var name = nameof(CompanyField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            CompanyField = FieldFactory.NewField(position, name, formattedValue);
        }

        private void SetPlantField(int position, string value)
        {
            var name = nameof(PlantField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            PlantField = FieldFactory.NewField(position, name, formattedValue);
        }

        public bool IsCorrupt()
        {
            return _fields.Any(f => f.IsCorrupted());
        }

        public IEnumerable<Exception> GetExceptions()
        {
            return _fields.Where(f => f.IsCorrupted()).Select(f => f.GetCorruptionException());
        }
    }
}
