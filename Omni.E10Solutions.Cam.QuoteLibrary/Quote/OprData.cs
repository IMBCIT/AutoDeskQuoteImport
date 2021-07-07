using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class OprData
    {
        public readonly string OpCode;
        public readonly string OpDesc;
        public readonly decimal ProdStandard;

        Field<decimal?> _oprField;

        public OprData(string opCode, string opDesc, Field<decimal?> oprField)
        {
            OpCode = opCode;
            OpDesc = opDesc;
            _oprField = oprField;
            ProdStandard = _oprField.GetValue() ?? 0m;
        }

        public bool IsSkippable()
        {
            return string.IsNullOrWhiteSpace(_oprField.GetTextValue());
        }
    }
}
