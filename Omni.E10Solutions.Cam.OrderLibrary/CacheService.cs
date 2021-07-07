using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class CacheService
    {
        Epicor10Cache _cache;

        public CacheService(Epicor10Cache cache)
        {
            _cache = cache;
        }

        public void Cache(CamductJobCollection jobs)
        {
            // * cache epicor data.
            var customerIds = jobs.GetDistinctCustomerIds();
            var partNumbers = jobs.GetDistinctPartNumbers();

            var customerErrors = _cache.LoadCustomerCache(customerIds);
            var partErrors = _cache.LoadPartsCache(partNumbers);

            // * register any errors that occurred during caching sequence.
            customerErrors.ToList().ForEach(e => jobs.RegisterCustomerError(e));
            partErrors.ToList().ForEach(e => jobs.RegisterPartError(e));
        }
    }
}
