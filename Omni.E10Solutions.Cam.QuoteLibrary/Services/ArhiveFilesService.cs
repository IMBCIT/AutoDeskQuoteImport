using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class ArchiveFilesService
    {
        string _archivesDirectoryPath;

        public ArchiveFilesService(string archivesDirectoryPath)
        {
            _archivesDirectoryPath = archivesDirectoryPath;
        }

        public string[] MoveToArchivesFolder(IEnumerable<string> filePaths)
        {
            var newFilePaths = new List<string>();

            using (new ImpersonateUser("Greg.Peart", "OmniDuct", "gpOmni17!"))
            {
                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);
                    var newFilePath = Path.Combine(_archivesDirectoryPath, fileName);
                    
                    if (File.Exists(newFilePath))
                        File.Delete(filePath);
                    else
                        File.Move(filePath, newFilePath);

                    newFilePaths.Add(newFilePath);               
                }
            }

            return newFilePaths.ToArray();
        }
    }
}
