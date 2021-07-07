using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class Epicor10Cache
    {
        Epicor10 _epicor;
        public Epicor10Cache(Epicor10 epicor)
        {
            _epicor = epicor;

            CustomerCache = new List<CustomerDataSet.CustomerRow>();
            ShipToCache = new List<CustomerDataSet.ShipToRow>();
            PartsCache = new List<PartListDataSet.PartListRow>();
        }

        public IEnumerable<CustomerDataSet.CustomerRow> CustomerCache { get; protected set; }
        public IEnumerable<CustomerDataSet.ShipToRow> ShipToCache { get; protected set; }
        public IEnumerable<PartListDataSet.PartListRow> PartsCache { get; protected set; }

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

        public IEnumerable<PartError> LoadPartsCache(IEnumerable<string> partNumbers)
        {
            if (partNumbers.Count() <= 0) return new List<PartError>();

            var pds = _epicor.GetPartsData(partNumbers);
            PartsCache = pds.PartList.Rows.Cast<PartListDataSet.PartListRow>();
            var cachedParts = PartsCache.Select(p => p.PartNum);
            var partErrors = partNumbers.Except(cachedParts).Select(p => new PartError(p));
            return partErrors;
        }
    }
}
