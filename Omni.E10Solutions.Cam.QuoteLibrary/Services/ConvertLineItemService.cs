using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class ConvertLineItemService : IConvertJobsToQuotesService
    {
        Epicor10 _epicor;
        Epicor10Cache _cache;

        public ConvertLineItemService(Epicor10 epicor, Epicor10Cache cache)
        {
            _epicor = epicor;
            _cache = cache;
        }

        public List<IQuote> Convert(CamductJobCollection jobs)
        {
            var udService = new UdService(_epicor);
            var quoteFactory = new QuoteFactory(_cache, udService);

            var quotes = new List<IQuote>();
            foreach (var job in jobs)
            {
                try
                {
                    var quote = quoteFactory.CreateQuote(job);
                    quotes.Add(quote);
                }
                catch (Exception ex)
                {
                    var quoteException = new QuoteMapException(job, ex);
                    job.RegisterException(quoteException);
                }
            }

            return quotes;
        }
    }
}
