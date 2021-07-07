using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class AdvancedServiceFactory
    {
        Epicor10 _epicor;
        Epicor10Cache _cache;
        ILog _logger;

        public AdvancedServiceFactory(Epicor10 epicor, Epicor10Cache cache, ILog logger)
        {
            _epicor = epicor;
            _cache = cache;
            _logger = logger;
        }

        public IConvertJobsToQuotesService CreateConvertService(ReadCamFileMode mode)
        {
            switch (mode)
            {
                case ReadCamFileMode.LineItem:
                    return new ConvertLineItemService(_epicor, _cache);
                case ReadCamFileMode.Rollup:
                    return new ConvertRollupService(_epicor, _cache);
                default:
                    throw new ReadCamFileModeInvalidException(mode);
            }
        }

        public IPersistQuoteService CreatePersistQuoteService(ReadCamFileMode mode)
        {
            switch (mode)
            {
                case ReadCamFileMode.LineItem:
                    return new PersistQuoteLineService(_epicor, _logger);
                case ReadCamFileMode.Rollup:
                    return new PersistQuoteRollupService(_epicor, _logger);
                default:
                    throw new ReadCamFileModeInvalidException(mode);
            }
        }

    }
}
