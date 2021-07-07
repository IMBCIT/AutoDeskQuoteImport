using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class CacheService
    {
        public Epicor10Cache _cache;

        public CacheService(Epicor10Cache cache)
        {
            _cache = cache;
        }

        public void Cache(CamductJobCollection jobs)
        {
            // * cache epicor data.
            var customerIds = jobs.GetDistinctCustomerIds();
            var nonDtlPartNumbers = jobs.GetDistinctNonDtlPartNumbers();
            var dtlPartNumbers = jobs.GetDistinctDtlPartNumbers();

            var customerErrors = _cache.LoadCustomerCache(customerIds);
            var nonDtlPartNumberErrors = _cache.LoadNonDtlPartsCache(nonDtlPartNumbers);
            var dtlPartNumberErrors = _cache.LoadDtlPartsCache(dtlPartNumbers);
            var partCostErrors = _cache.LoadPartCostsCache(nonDtlPartNumbers, jobs.Plant);

            // * register any errors that occurred during caching sequence.
            customerErrors.ToList().ForEach(e => jobs.RegisterCustomerError(e));
            nonDtlPartNumberErrors.ToList().ForEach(e => jobs.RegisterPartError(e));
            dtlPartNumberErrors.ToList().ForEach(e => jobs.RegisterPartError(e));
            partCostErrors.ToList().ForEach(e => jobs.RegisterPartCostError(e));
        }
    }
}
