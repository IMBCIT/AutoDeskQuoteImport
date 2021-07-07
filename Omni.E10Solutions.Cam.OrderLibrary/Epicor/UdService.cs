using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class UdService
    {
        Epicor10 _epicor;

        public UdService(Epicor10 epicor)
        {
            _epicor = epicor;
        }

        public void PrepDataSet(UpdExtSalesOrderDataSet qds)
        {
            _epicor.MakeDataSetUDCompatible(qds);
        }

    }
}
