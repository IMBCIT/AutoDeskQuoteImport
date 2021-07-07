using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class TypeOData
    {
        List<IField> _fields = new List<IField>();

        public Field<string> CustomerIdField { get; protected set; }
        public Field<string> ProjectNameField { get; protected set; }
        public Field<string> JobFileNameField { get; protected set; }
        public Field<string> DateField { get; protected set; }
        public Field<string> TimeField { get; protected set; }
        public Field<string> EpicorGroupField { get; protected set; }
        public Field<string> ItemNoField { get; protected set; }
        public Field<decimal> QtyField { get; protected set; }
        public Field<string> ItemAliasField { get; protected set; }

        public TypeOData(string[] data)
        {
            if (data.Length < 9)
                throw new NotEnoughColumnsException("O");

            SetCustomerIdField(0, data[0]);
            SetProjectNameField(1, data[1]);
            SetJobFileNameField(2, data[2]);
            SetDateField(3, data[3]);
            SetTimeField(4, data[4]);
            SetEpicorGroupField(5, data[5]);
            SetItemNoField(6, data[6]);
            SetQtyField(7, data[7]);
            SetItemAliasField(8, data[8]);

            _fields.Add(CustomerIdField);
            _fields.Add(ProjectNameField);
            _fields.Add(JobFileNameField);
            _fields.Add(DateField);
            _fields.Add(TimeField);
            _fields.Add(EpicorGroupField);
            _fields.Add(ItemNoField);
            _fields.Add(QtyField);
            _fields.Add(ItemAliasField);
        }

        public bool IsCorrupt()
        {
            return _fields.Any(f => f.IsCorrupted());
        }

        public IEnumerable<Exception> GetExceptions()
        {
            return _fields.Where(f => f.IsCorrupted()).Select(f => f.GetCorruptionException());
        }

        void SetCustomerIdField(int position, string value)
        {
            var name = nameof(CustomerIdField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            CustomerIdField = new Field<string>(position, name, formattedValue);
        }

        void SetProjectNameField(int position, string value)
        {
            var name = nameof(ProjectNameField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            ProjectNameField = new Field<string>(position, name, formattedValue);
        }

        void SetJobFileNameField(int position, string value)
        {
            var name = nameof(JobFileNameField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            JobFileNameField = new Field<string>(position, name, formattedValue);
        }

        void SetDateField(int position, string value)
        {
            var name = nameof(DateField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            DateField = new Field<string>(position, name, formattedValue);
        }

        void SetTimeField(int position, string value)
        {
            var name = nameof(TimeField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            TimeField = new Field<string>(position, name, formattedValue);
        }

        void SetEpicorGroupField(int position, string value)
        {
            var name = nameof(EpicorGroupField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            EpicorGroupField = new Field<string>(position, name, formattedValue);
        }

        void SetItemNoField(int position, string value)
        {
            var name = nameof(ItemNoField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            ItemNoField = new Field<string>(position, name, formattedValue);
        }

        void SetQtyField(int position, string value)
        {
            var name = nameof(QtyField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            QtyField = new Field<decimal>(position, name, formattedValue);
        }

        void SetItemAliasField(int position, string value)
        {
            var name = nameof(ItemAliasField).Replace("Field", ""); ;
            var formattedValue = value.StripQuotationMarks().ToUpper();
            ItemAliasField = new Field<string>(position, name, formattedValue);
        }

    }
}
