using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class CamductJobPartNumber
    {
        public readonly string PartNumber;
        public readonly CamductJob Job;

        public CamductJobPartNumber(string partNumber, CamductJob job)
        {
            PartNumber = partNumber;
            Job = job;
        }
    }
}
