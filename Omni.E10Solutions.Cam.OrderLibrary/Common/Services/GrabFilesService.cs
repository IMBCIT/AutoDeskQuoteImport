using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class GrabFilesService
    {
        public RawFileDataDictionary Grab(DirectoryParameter dir)
        {
            using (new ImpersonateUser("epicadmin", "OMNI", "0mni3pic539#"))
            {
                // *get file paths from the directory
                var filePaths = GetFiles(dir);
                if (filePaths.Count() == 0) return null;

                // * move the files to the in process dir 
                var fileService = new MoveFilesService(dir);
                filePaths = fileService.Process(filePaths); // returns new file paths

                // * read the file data into memory
                var rawFileDataDictionary = ReadFiles(filePaths);

                return rawFileDataDictionary;
            }
        }

        string[] GetFiles(DirectoryParameter dir)
        {
            var directory = new DirectoryInfo(dir.SourceDirectory);
            var filePaths = directory.GetFiles(dir.SearchPattern, dir.SearchOption)
                .Select(file => file.FullName).ToArray();
            return filePaths;
        }

        RawFileDataDictionary ReadFiles(IEnumerable<string> filePaths)
        {
            var fileDataDictionary = new RawFileDataDictionary();
            foreach (string path in filePaths)
            {
                var pathData = new List<string[]>(); // read in the file lines
                using (var reader = new StreamReader(path))
                {
                    reader.ReadLine(); // skip first line
                    while (!reader.EndOfStream)
                    {
                        string[] fileLineFields = reader.ReadLine().Split(',');
                        pathData.Add(fileLineFields);
                    }
                }
                fileDataDictionary.Add(path, pathData); // match the file to its lines
            }

            return fileDataDictionary;
        }
    }
}
