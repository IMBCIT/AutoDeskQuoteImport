using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class UdService
    {
        Epicor10 _epicor;

        public UdService(Epicor10 epicor)
        {
            _epicor = epicor;
        }

        public void PrepDataSet(UpdExtJobEntryDataSet jds)
        {
            _epicor.MakeDataSetUDCompatible(jds);
        }

    }
}
