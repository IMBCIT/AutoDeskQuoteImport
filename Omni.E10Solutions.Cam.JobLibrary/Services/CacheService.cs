using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
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
            var nonDtlPartNumbers = jobs.GetDistinctNonDtlPartNumbers();
            var dtlPartNumbers = jobs.GetDistinctDtlPartNumbers();

            var nonDtlPartNumberErrors = _cache.LoadNonDtlPartsCache(nonDtlPartNumbers);
            var dtlPartNumberErrors = _cache.LoadDtlPartsCache(dtlPartNumbers);
            var partCostErrors = _cache.LoadPartCostsCache(nonDtlPartNumbers, jobs.Plant);

            // * register any errors that occurred during caching sequence.
            nonDtlPartNumberErrors.ToList().ForEach(e => jobs.RegisterPartError(e));
            dtlPartNumberErrors.ToList().ForEach(e => jobs.RegisterPartError(e));
            partCostErrors.ToList().ForEach(e => jobs.RegisterPartCostError(e));
        }
    }
}
