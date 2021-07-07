using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    interface IOrder
    {
        CamductJob Job { get; }

        UpdExtSalesOrderDataSet GetOrderDataSet();
        int GetOrderNum();
    }
}
