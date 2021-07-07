using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    interface IQuoteFactory
    {
        IQuote CreateQuote(CamductJob job);
    }
}
