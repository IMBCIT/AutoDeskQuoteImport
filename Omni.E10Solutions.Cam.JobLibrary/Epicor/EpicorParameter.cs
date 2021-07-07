using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class EpicorParameter
    {
        public readonly string Plant, Company;
        public readonly string AppServerUrl;

        public EpicorParameter(string appServerUrl, string company, string plant)
        {
            Company = company;
            Plant = plant;
            AppServerUrl = appServerUrl;
        }
    }
}
