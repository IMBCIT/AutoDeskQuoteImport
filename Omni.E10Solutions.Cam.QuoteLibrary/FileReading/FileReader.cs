using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class FileReader
    {
        public RawFileDataDictionary Read(IEnumerable<string> filePaths)
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
