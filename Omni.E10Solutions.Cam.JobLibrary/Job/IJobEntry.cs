using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.BO;
using Ice.BO;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public interface IJobEntry
    {
        CamductJob Job { get; }

        UpdExtJobEntryDataSet GetJobEntryDataSet();
        string GetJobNum();
        void SetJobNum(string jobNum);
        UpdExtUD03DataSet GetReportingDataSet();
    }
}
