using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class TypeSData
    {
        public Field<string> JobNameField { get; protected set; }
        public Field<string> DateField { get; protected set; }
        public Field<string> TimeField { get; protected set; }
        public Field<string> LinearThicknessField { get; protected set; }
        public Field<string> MaterialField { get; protected set; }
        public Field<string> GaugeField { get; protected set; }
        public Field<decimal?> BlankWeightField { get; protected set; }
        public Field<string> InsulationField { get; protected set; }
        public Field<decimal?> CutAreaField { get; protected set; }
        public Field<decimal?> ScrapWtField { get; protected set; }
        public Field<decimal?> ScrapSFField { get; protected set; }

        void SetJobNameField(int position, string value)
        {
            var name = nameof(JobNameField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            JobNameField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetDateField(int position, string value)
        {
            var name = nameof(DateField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            DateField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetTimeField(int position, string value)
        {
            var name = nameof(TimeField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            TimeField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetLinearThicknessField(int position, string value)
        {
            var name = nameof(LinearThicknessField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            LinearThicknessField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetMaterialField(int position, string value)
        {
            var name = nameof(MaterialField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            MaterialField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetGaugeField(int position, string value)
        {
            var name = nameof(GaugeField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            GaugeField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetBlankWeightField(int position, string value)
        {
            var name = nameof(BlankWeightField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            BlankWeightField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetInsulationField(int position, string value)
        {
            var name = nameof(InsulationField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            InsulationField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetCutAreaField(int position, string value)
        {
            var name = nameof(CutAreaField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            CutAreaField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetScrapWtField(int position, string value)
        {
            var name = nameof(ScrapWtField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ScrapWtField = FieldFactory.NewNullableDecimalField(position, name, formattedValue, 0);
        }

        void SetScrapSFField(int position, string value)
        {
            var name = nameof(ScrapSFField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks();
            ScrapSFField = FieldFactory.NewNullableDecimalField(position, name, formattedValue, 0);
        }

        public TypeSData(string[] data)
        {
            if (data.Length < 11)
                throw new NotEnoughColumnsException("S");

            SetJobNameField(0, data[0]);
            SetDateField(1, data[1]);
            SetTimeField(2, data[2]);
            SetInsulationField(3, data[3]);
            SetMaterialField(4, data[4]);
            SetGaugeField(5, data[5]);
            SetLinearThicknessField(6, data[6]);
            SetBlankWeightField(7, data[7]);
            SetCutAreaField(8, data[8]);
            SetScrapWtField(9, data[9]);
            SetScrapSFField(10, data[10]);

            _fields.Add(JobNameField);
            _fields.Add(DateField);
            _fields.Add(TimeField);
            _fields.Add(InsulationField);
            _fields.Add(MaterialField);
            _fields.Add(GaugeField);
            _fields.Add(LinearThicknessField);
            _fields.Add(BlankWeightField);
            _fields.Add(CutAreaField);
            _fields.Add(ScrapWtField);
            _fields.Add(ScrapSFField);
        }

        List<IField> _fields = new List<IField>();

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
