using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class Epicor10Cache
    {
        Epicor10 _epicor;
        public Epicor10Cache(Epicor10 epicor)
        {
            _epicor = epicor;

            CustomerCache = new List<CustomerDataSet.CustomerRow>();
            ShipToCache = new List<CustomerDataSet.ShipToRow>();
            NonDtlPartCache_List = new List<PartListDataSet.PartListRow>();
            NonDtlPartCache = new List<PartDataSet.PartRow>();
            DtlPartCache = new List<PartListDataSet.PartListRow>();
            PartCostCache = new List<PartCostListDataSet.PartCostListRow>();
        }

        public IEnumerable<CustomerDataSet.CustomerRow> CustomerCache { get; protected set; }
        public IEnumerable<CustomerDataSet.ShipToRow> ShipToCache { get; protected set; }
        IEnumerable<PartListDataSet.PartListRow> NonDtlPartCache_List { get; set; }
        public IEnumerable<PartDataSet.PartRow> NonDtlPartCache { get; set; }
        public IEnumerable<PartListDataSet.PartListRow> DtlPartCache { get; protected set; }
        public IEnumerable<PartCostListDataSet.PartCostListRow> PartCostCache { get; protected set; }

        public IEnumerable<Tuple<string, Exception>> CacheCustomersFromStore(IEnumerable<string> customers)
        {
            var customerErrors = new List<Tuple<string, Exception>>();

            var custRows = new List<CustomerDataSet.CustomerRow>();
            var shipRows = new List<CustomerDataSet.ShipToRow>();
            foreach (var customer in customers)
            {
                try
                {
                    var cds = _epicor.GetCustomerData(customer);
                    var customerRow = cds.Customer.Rows.Cast<CustomerDataSet.CustomerRow>().First();
                    custRows.Add(customerRow);
                    shipRows.AddRange(cds.ShipTo.Rows.Cast<CustomerDataSet.ShipToRow>());
                }
                catch (Exception ex)
                {
                    customerErrors.Add(new Tuple<string, Exception>(customer, ex));
                }
            }

            CustomerCache = custRows;
            ShipToCache = shipRows;
            return customerErrors;
        }

        public IEnumerable<CustomerError> LoadCustomerCache(IEnumerable<string> customerIds)
        {
            var customerErrors = new List<CustomerError>();

            var custRows = new List<CustomerDataSet.CustomerRow>();
            var shipRows = new List<CustomerDataSet.ShipToRow>();
            foreach (var customerId in customerIds)
            {
                try
                {
                    var cds = _epicor.GetCustomerData(customerId);
                    var customerRow = cds.Customer.Rows.Cast<CustomerDataSet.CustomerRow>().First();
                    custRows.Add(customerRow);
                    shipRows.AddRange(cds.ShipTo.Rows.Cast<CustomerDataSet.ShipToRow>());
                }
                catch (Exception ex)
                {
                    customerErrors.Add(new CustomerError(customerId, ex));
                }
            }

            CustomerCache = custRows;
            ShipToCache = shipRows;
            return customerErrors;
        }

        public IEnumerable<string> CachePartsFromStore(IEnumerable<string> parts)
        {
            if (parts.Count() <= 0) return new List<string>();

            var pds = _epicor.GetPartsListData(parts);
            NonDtlPartCache_List = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = NonDtlPartCache_List.Select(p => p.PartNum);
            var partErrors = parts.Except(cachedParts);
            return partErrors;
        }

        public IEnumerable<string> CacheDtlPartsFromStore(IEnumerable<string> dtlParts)
        {
            if (dtlParts.Count() <= 0) return new List<string>();

            var pds = _epicor.GetPartsListData(dtlParts);
            DtlPartCache = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = DtlPartCache.Select(p => p.PartNum);
            var partErrors = dtlParts.Except(cachedParts);
            return partErrors;
        }

        public IEnumerable<PartError> LoadNonDtlPartsCache(IEnumerable<string> partNumbers)
        {
            return LoadNonDtlPartsFromRowData(partNumbers);
        }

        private IEnumerable<PartError> LoadNonDtlPartsFromListData(IEnumerable<string> pns)
        {
            if (pns.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsListData(pns);
            NonDtlPartCache_List = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = NonDtlPartCache_List.Select(p => p.PartNum);
            var partErrors = pns.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }

        private IEnumerable<PartError> LoadNonDtlPartsFromRowData(IEnumerable<string> pns)
        {
            if (pns.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsDataSet(pns);
            NonDtlPartCache = pds.Part.Rows.Cast<PartDataSet.PartRow>();
            var cachedParts = NonDtlPartCache.Select(p => p.PartNum);
            var partErrors = pns.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }

        public IEnumerable<PartError> LoadDtlPartsCache(IEnumerable<string> partNumbers)
        {
            if (partNumbers.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsListData(partNumbers);
            DtlPartCache = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = DtlPartCache.Select(p => p.PartNum);
            var partErrors = partNumbers.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }

        public IEnumerable<string> CachePartCostsFromStore(IEnumerable<string> parts, string plant)
        {
            if (parts.Count() <= 0) return new List<string>();

            var pcds = _epicor.GetPartCostsData(parts, plant);
            PartCostCache = pcds.PartCostList.Rows.Cast<PartCostListDataSet.PartCostListRow>();
            var cachedParts = PartCostCache.Select(p => p.PartNum);
            var partCostErrors = parts.Except(cachedParts);
            return partCostErrors;
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
