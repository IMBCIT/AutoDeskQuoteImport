using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class PartError
    {
        public readonly string PartNumber;
        public readonly Exception Exception;

        public PartError(string partNumber)
        {
            PartNumber = partNumber;
            Exception = new Exception("Part Number " + partNumber + " does not exist in Epicor.");
        }
    }
}
