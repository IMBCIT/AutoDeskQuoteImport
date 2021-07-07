using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class Order : IOrder
    {
        public Order(CamductJob job)
        {
            Job = job;
            OrderDataSet = new UpdExtSalesOrderDataSet();
        }

        public CamductJob Job { get; protected set; }

        public UpdExtSalesOrderDataSet OrderDataSet { get; set; }

        public UpdExtSalesOrderDataSet.OrderHedRow NewHedRow()
        {
            return this.OrderDataSet.OrderHed.NewOrderHedRow();
        }

        public UpdExtSalesOrderDataSet.OrderDtlRow NewDtlRow()
        {
            return this.OrderDataSet.OrderDtl.NewOrderDtlRow();
        }

        public void AddHedRow(UpdExtSalesOrderDataSet.OrderHedRow row)
        {
            this.OrderDataSet.OrderHed.AddOrderHedRow(row);
        }

        public void AddDtlRow(UpdExtSalesOrderDataSet.OrderDtlRow row)
        {
            this.OrderDataSet.OrderDtl.AddOrderDtlRow(row);
        }

        public UpdExtSalesOrderDataSet GetOrderDataSet()
        {
            return OrderDataSet;
        }

        public int GetOrderNum()
        {
            var hed = OrderDataSet.OrderHed.Rows
                .Cast<UpdExtSalesOrderDataSet.OrderHedRow>()
                .FirstOrDefault();

            if (hed != null)
            {
                return hed.OrderNum;
            }

            return 0;
        }

        public void MakeUDCompatible(UdService udService)
        {
            udService.PrepDataSet(OrderDataSet);
        }
    }
}
