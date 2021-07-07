using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HedRow = Erp.BO.UpdExtSalesOrderDataSet.OrderHedRow;
using DtlRow = Erp.BO.UpdExtSalesOrderDataSet.OrderDtlRow;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class OrderFactory
    {
        Epicor10Cache _cache;
        UdService _udService;

        public OrderFactory(Epicor10Cache cache, UdService udService)
        {
            _cache = cache;
            _udService = udService;
        }

        public IOrder CreateOrder(CamductJob job)
        {
            var order = new Order(job);
            order.MakeUDCompatible(_udService);

            // create header
            var hed = CreateOrderHedRow(order, job);
            order.AddHedRow(hed);

            foreach (var oFileLine in job.OFile)
            {
                var oDataConverter = new TypeODataConverter(oFileLine);
                var dtl = CreateOrderDtlRow(order, oDataConverter);
                order.AddDtlRow(dtl);
            }

            return order;
        }

        HedRow CreateOrderHedRow(Order order, CamductJob job)
        {
            var hed = order.NewHedRow();

            var firstOLine = job.OFile.FirstOrDefault();
            if (firstOLine == null)
            {
                throw new NoFirstOLineException(job.Name);
            }

            var oDataConverter = new TypeODataConverter(firstOLine);

            hed.CustomerCustID = job.GetCustomerId();
            hed.PONum = oDataConverter.GetHedPoNum();
            hed.OrderComment = job.Name;

            var customer = _cache.CustomerCache.FirstOrDefault(c => c.CustID == hed.CustomerCustID);
            if (customer != null)
            {
                hed.CustNum = customer.CustNum;
                hed.TermsCode = customer.TermsCode;
                hed.ShipViaCode = customer.ShipViaCode;
                hed.BTCustID = customer.CustID;
                hed.BTCustNum = customer.CustNum;
            }
            else
            {
                throw new NoCustomerFoundInCacheException(job.GetCustomerId());
            }

            var shipto = _cache.ShipToCache.FirstOrDefault(s => s.CustNum == hed.CustNum);
            if (shipto != null)
            {
                hed.ShipToCustNum = shipto.CustNum;
                hed.ShipToCustId = shipto.CustNumCustID;
            } // skips these, they are automatically filled out by Epicor using the customer if not present.

            return hed;
        }

        DtlRow CreateOrderDtlRow(Order order, TypeODataConverter oDataConverter)
        {
            var dtl = order.NewDtlRow();

            dtl.ProdCode = oDataConverter.GetDtlProdCode();
            dtl.POLine = oDataConverter.GetPOLine();
            dtl.SellingQuantity = oDataConverter.GetDtlSellingQty();
            dtl.PartNum = oDataConverter.GetDtlPartNum();

            var part = _cache.PartsCache.FirstOrDefault(p => p.PartNum == dtl.PartNum);
            if (part != null)
            {
                dtl.LineDesc = part.PartDescription;
                dtl.SalesUM = part.IUM;
                dtl.IUM = part.IUM;
            }
            else
            {
                throw new NoPartFoundInCacheException(dtl.PartNum);
            }

            return dtl;
        }
    }
}
