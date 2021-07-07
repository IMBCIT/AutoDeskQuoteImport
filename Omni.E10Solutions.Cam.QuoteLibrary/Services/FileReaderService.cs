using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class FileReaderService
    {
        public RawFileDataDictionary Read(string[] filePaths)
        {
            using (new ImpersonateUser("Greg.Peart", "OmniDuct", "gpOmni17!"))
            {
                var reader = new FileReader();
                var rawFileDataDictionary = reader.Read(filePaths);
                return rawFileDataDictionary;
            }
        }
    }
}
