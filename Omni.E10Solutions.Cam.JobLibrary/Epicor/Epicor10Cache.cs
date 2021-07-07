using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class Epicor10Cache
    {
        Epicor10 _epicor;
        public Epicor10Cache(Epicor10 epicor)
        {
            _epicor = epicor;

            NonDtlPartCache = new List<PartListDataSet.PartListRow>();
            DtlPartCache = new List<PartListDataSet.PartListRow>();
        }

        public IEnumerable<PartListDataSet.PartListRow> NonDtlPartCache { get; protected set; }
        public IEnumerable<PartListDataSet.PartListRow> DtlPartCache { get; protected set; }
        public IEnumerable<PartCostListDataSet.PartCostListRow> PartCostCache { get; protected set; }

        public IEnumerable<PartError> LoadNonDtlPartsCache(IEnumerable<string> partNumbers)
        {
            if (partNumbers.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsData(partNumbers);
            NonDtlPartCache = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = NonDtlPartCache.Select(p => p.PartNum);
            var partErrors = partNumbers.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }

        public IEnumerable<PartError> LoadDtlPartsCache(IEnumerable<string> partNumbers)
        {
            if (partNumbers.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsData(partNumbers);
            DtlPartCache = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = DtlPartCache.Select(p => p.PartNum);
            var partErrors = partNumbers.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }

        public IEnumerable<PartCostError> LoadPartCostsCache(IEnumerable<string> partNumbers, string plant)
        {
            if (partNumbers.Count() <= 0) return new List<PartCostError>();

            var pcds = _epicor.GetPartCostsData(partNumbers, plant);
            PartCostCache = pcds.PartCostList.Rows.Cast<PartCostListDataSet.PartCostListRow>();
            var cachedParts = PartCostCache.Select(p => p.PartNum);
            var partCostErrors = partNumbers.Except(cachedParts).Select(p => new PartCostError(p, plant));
            return partCostErrors;
        }
    }
}
