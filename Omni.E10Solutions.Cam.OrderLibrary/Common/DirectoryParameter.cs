using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    public class DirectoryParameter
    {
        public readonly string SourceDirectory;
        public readonly string InProcessDirectory;
        public readonly string ArchivesDirectory;
        public readonly string ErrorDirectory;
        public readonly string SearchPattern;
        public readonly SearchOption SearchOption;

        public DirectoryParameter(string sourceDirectory, string inProcessDirectory, string archivesDirectory, string errorDirectory, string searchPattern, SearchOption searchOption)
        {
            SourceDirectory = sourceDirectory;
            InProcessDirectory = inProcessDirectory;
            ArchivesDirectory = archivesDirectory;
            ErrorDirectory = errorDirectory;
            SearchPattern = searchPattern;
            SearchOption = searchOption;
        }
    }
}
