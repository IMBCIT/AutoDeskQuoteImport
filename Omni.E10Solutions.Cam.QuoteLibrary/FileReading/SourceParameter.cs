using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class SourceParameter
    {
        public readonly string SourceDirectory;
        public readonly string ArchivesDirectory;
        public readonly string ErrorDirectory;
        public readonly string SearchPattern;
        public readonly SearchOption SearchOption;

        public SourceParameter(string sourceDirectory, string archivesDirectory, string errorDirectory, string searchPattern, SearchOption searchOption)
        {
            SourceDirectory = sourceDirectory;
            ArchivesDirectory = archivesDirectory;
            ErrorDirectory = errorDirectory;
            SearchPattern = searchPattern;
            SearchOption = searchOption;
        }
    }
}
