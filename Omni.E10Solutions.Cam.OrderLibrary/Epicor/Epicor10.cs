using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using Erp.Proxy.BO;
using Ice.Common;
using Ice.Core;
using Ice.Lib.Framework;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class Epicor10
    {
        Session _session;
        SalesOrderImpl _soBo;
        CustomerImpl _customerBo;
        PartImpl _partBo;

        public Epicor10(string appServerUrl)
        {
            _session = new Session("Epicor.One", "esOmni01!", appServerUrl);

            _soBo = WCFServiceSupport.CreateImpl<SalesOrderImpl>(_session, SalesOrderImpl.UriPath);
            _customerBo = WCFServiceSupport.CreateImpl<CustomerImpl>(_session, CustomerImpl.UriPath);
            _partBo = WCFServiceSupport.CreateImpl<PartImpl>(_session, PartImpl.UriPath);
        }

        public void DeleteOrder(int ordernum)
        {
            _soBo.DeleteByID(ordernum);
        }

        public CustomerDataSet GetCustomerData(string customerId)
        {
            return _customerBo.GetByCustID(customerId, true);
        }

        public PartDataSet GetPartData(string partnum)
        {
            return _partBo.GetByID(partnum);
        }

        public PartListDataSet GetPartsData(IEnumerable<string> parts)
        {
            var wheres = parts.Select(p => "partnum = '" + p + "'");
            var whereClause = string.Join(" or ", wheres);
            bool b;
            var pds = _partBo.GetList(whereClause, 0, 0, out b);
            return pds;
        }

        public void MakeDataSetUDCompatible(UpdExtSalesOrderDataSet ds)
        {
            this.CommitOrder(ds); // silly I know.
        }

        public string CommitOrder(UpdExtSalesOrderDataSet ds)
        {
            bool errors = false;
            try
            {
                var quoteBoErrors = _soBo.UpdateExt(ds, true, false, out errors);
                if (quoteBoErrors.BOUpdError.Rows.Count > 0)
                {
                    return quoteBoErrors.BOUpdError.Rows[0]["ErrorText"].ToString();
                }
            }
            catch //(BusinessObjectException ex)
            {
            }

            return string.Empty;
        }

        public void CloseSession()
        {
            _session.Dispose();
        }
    }
}
