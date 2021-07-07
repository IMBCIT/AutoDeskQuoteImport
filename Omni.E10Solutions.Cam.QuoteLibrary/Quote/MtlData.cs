using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class MtlData
    {
        public static MtlData NewLineItemMtl(string partNumber, Field<decimal?> mtlField, string ium, string line)
        {
            return new MtlData(partNumber, mtlField, ium) { Line = line };
        }

        public static MtlData NewRollupMtl(string partNumber, Field<decimal?> mtlField, decimal oQty, decimal qtySum, string ium)
        {
            return new MtlData(partNumber, mtlField, oQty, qtySum, ium);
        }

        public readonly string PartNumber;
        public readonly decimal QtyPer;
        public readonly string IUM;
        public string Line { get; private set; }

        Field<decimal?> _mtlField;

        private MtlData(string partNumber, Field<decimal?> mtlField, string ium)
        {
            PartNumber = partNumber;
            IUM = ium;

            _mtlField = mtlField;

            QtyPer = _mtlField.GetValue() ?? 0m;
        }

        private MtlData(string partNumber, Field<decimal?> mtlField, decimal oQty, decimal qtySum, string ium)
        {
            PartNumber = partNumber;
            IUM = ium;

            _mtlField = mtlField;

            QtyPer = ((_mtlField.GetValue() ?? 0m) * oQty) / qtySum;
        }

        public bool IsSkippable()
        {
            return string.IsNullOrWhiteSpace(_mtlField.GetTextValue());
        }

        public Field<decimal?> GetField()
        {
            return _mtlField;
        }
    }
}
