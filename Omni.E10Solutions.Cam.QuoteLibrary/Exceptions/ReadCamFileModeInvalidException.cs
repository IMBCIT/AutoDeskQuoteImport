using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class ReadCamFileModeInvalidException : Exception
    {
        public ReadCamFileModeInvalidException(CamConversionMode mode)
            : base("The ReadCamFileMode is invalid. Mode is " + mode.ToString() + ".")
        {

        }
    }
}
