using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class RollupOprData
    {
        public readonly string OpCode;
        public readonly string OpDesc;
        public readonly decimal ProdStandard;

        public RollupOprData(string opCode, string opDesc, decimal prodStandard)
        {
            OpCode = opCode;
            OpDesc = opDesc;
            ProdStandard = prodStandard;
        }
    }
}
