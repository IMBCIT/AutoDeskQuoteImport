using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class CamductJobBuilder
    {
        public CamductJob BuildCamductJob(string jobName, string plant, RawFileDataDictionary jobFileDataDictionary)
        {
            FileTypeO oFile = null;

            foreach (var file in jobFileDataDictionary)
            {
                var filePath = file.Key;
                var fileType = filePath.Substring(filePath.Length - 6);
                var fileData = file.Value;

                oFile = BuildOFile(filePath, fileData);
            }

            return new CamductJob(jobName, plant, oFile);
        }

        FileTypeO BuildOFile(string filePath, IEnumerable<string[]> fileData)
        {
            try
            {
                var oFile = new FileTypeO(filePath);
                foreach (var line in fileData)
                {
                    oFile.Add(new TypeOData(line));
                }
                return oFile;
            }
            catch (Exception ex)
            {
                var oFile = new FileTypeO(filePath);
                oFile.RegisterNewException(ex);
                return oFile;
            }
        }
    }
}
