using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class CorruptedFileTypeA : FileTypeA
    {
        public readonly Exception Exception;

        public CorruptedFileTypeA(string path, Exception ex)
            : base(path)
        {
            Exception = ex;
        }
    }
}
