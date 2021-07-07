using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class MtlData
    {
        public readonly string PartNumber;
        public readonly decimal QtyPer;
        public readonly string IUM;

        Field<decimal?> _mtlField;

        public MtlData(string partNumber, Field<decimal?> mtlField, string ium)
        {
            PartNumber = partNumber;
            IUM = ium;

            _mtlField = mtlField;

            QtyPer = _mtlField.GetValue() ?? 0m;
        }

        public bool IsSkippable()
        {
            return string.IsNullOrWhiteSpace(_mtlField.GetTextValue());
        }
    }
}
