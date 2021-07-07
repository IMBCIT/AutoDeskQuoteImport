using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class GetFilesService
    {
        public string[] GetFiles(SourceParameter source)
        {
            using (new ImpersonateUser("Greg.Peart", "OmniDuct", "gpOmni17!"))
            {
                var directory = new DirectoryInfo(source.SourceDirectory);
                var filePaths = directory.GetFiles(source.SearchPattern, source.SearchOption)
                    .Select(file => file.FullName).ToArray();
                return filePaths;
            }
        }
    }
}
