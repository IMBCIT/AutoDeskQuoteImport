using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class EpicorServiceFactory
    {
        Epicor10 _epicor;
        Epicor10Cache _cache;
        EpicorParameter _epi;
        ILog _logger;
        CamConversionMode _mode;

        public EpicorServiceFactory(EpicorParameter epi, ILog logger, CamConversionMode mode)
        {
            _epi = epi;
            _logger = logger;
            _mode = mode;
        }

        public void Connect()
        {
            _epicor = new Epicor10(_epi);
            _cache = new Epicor10Cache(_epicor);
        }

        public void Disconnect()
        {
            if (_epicor != null)
            {
                _epicor.CloseSession();
                _epicor = null;
            }
        }

        public IConvertJobsToQuotesService CreateConvertService()
        {
            switch (_mode)
            {
                case CamConversionMode.LineItem:
                    return new ConvertLineItemService(_epicor, _cache);
                case CamConversionMode.Rollup:
                    return new ConvertRollupService(_epicor, _cache);
                default:
                    throw new ReadCamFileModeInvalidException(_mode);
            }
        }

        public IPersistQuoteService CreatePersistQuoteService()
        {
            switch (_mode)
            {
                case CamConversionMode.LineItem:
                    return new PersistQuoteLineService(_epicor, _logger);
                case CamConversionMode.Rollup:
                    return new PersistQuoteRollupService(_epicor, _logger);
                default:
                    throw new ReadCamFileModeInvalidException(_mode);
            }
        }

        public CacheService CreateCacheService()
        {
            return new CacheService(_cache);
        }

    }
}
