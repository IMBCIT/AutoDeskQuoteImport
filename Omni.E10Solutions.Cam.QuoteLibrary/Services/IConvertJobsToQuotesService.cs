using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public interface IConvertJobsToQuotesService
    {
        List<IQuote> Convert(CamductJobCollection jobs);
    }
}
