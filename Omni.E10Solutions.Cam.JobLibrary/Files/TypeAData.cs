using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class TypeAData
    {
        public Field<string> JobNameField { get; protected set; }
        public Field<string> DateField { get; protected set; }
        public Field<string> TimeField { get; protected set; }
        public Field<string> ItemNoField { get; protected set; }
        public Field<string> AncillaryNameField { get; protected set; }
        public Field<string> AncillaryPartNoField { get; protected set; }
        public Field<decimal?> AncillaryLengthField { get; protected set; }
        public Field<decimal?> AncillaryQtyField { get; protected set; }

        void SetJobNameField(int position, string value)
        {
            var name = nameof(JobNameField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            JobNameField = FieldFactory.NewField(position, name, formattedValue);
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

        void SetItemNoField(int position, string value)
        {
            var name = nameof(ItemNoField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            ItemNoField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetAncillaryNameField(int position, string value)
        {
            var name = nameof(AncillaryNameField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            AncillaryNameField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetAncillaryPartNoField(int position, string value)
        {
            var name = nameof(AncillaryPartNoField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            AncillaryPartNoField = FieldFactory.NewField(position, name, formattedValue);
        }

        void SetAncillaryLengthField(int position, string value)
        {
            var name = nameof(AncillaryLengthField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            AncillaryLengthField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        void SetAncillaryQtyField(int position, string value)
        {
            var name = nameof(AncillaryQtyField).Replace("Field", "");
            var formattedValue = value.StripQuotationMarks();
            AncillaryQtyField = FieldFactory.NewNullableDecimalField(position, name, formattedValue);
        }

        public TypeAData(string[] data)
        {
            if (data.Length < 8)
                throw new NotEnoughColumnsException("A");

            SetJobNameField(0, data[0]);
            SetDateField(1, data[1]);
            SetTimeField(2, data[2]);
            SetItemNoField(3, data[3]);
            SetAncillaryNameField(4, data[4]);
            SetAncillaryPartNoField(5, data[5]);
            SetAncillaryLengthField(6, data[6]);
            SetAncillaryQtyField(7, data[7]);

            _fields.Add(JobNameField);
            _fields.Add(DateField);
            _fields.Add(TimeField);
            _fields.Add(ItemNoField);
            _fields.Add(AncillaryNameField);
            _fields.Add(AncillaryPartNoField);
            _fields.Add(AncillaryLengthField);
            _fields.Add(AncillaryQtyField);
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
