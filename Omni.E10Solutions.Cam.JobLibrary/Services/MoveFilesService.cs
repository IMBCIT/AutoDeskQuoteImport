using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class MoveFilesService
    {
        DirectoryParameter _dir;

        public MoveFilesService(DirectoryParameter dir)
        {
            _dir = dir;
        }

        public string[] Archive(IEnumerable<string> filePaths)
        {
            return Move(filePaths, _dir.ArchivesDirectory);
        }

        public string[] Process(IEnumerable<string> filePaths)
        {
            return Move(filePaths, _dir.InProcessDirectory);
        }

        string[] Move(IEnumerable<string> filePaths, string destinationDir)
        {
            var newFilePaths = new List<string>();

            using (new ImpersonateUser("epicadmin", "OMNI", "0mni3pic539#"))
            {
                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileName(filePath);
                    var newFilePath = Path.Combine(destinationDir, fileName);

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
