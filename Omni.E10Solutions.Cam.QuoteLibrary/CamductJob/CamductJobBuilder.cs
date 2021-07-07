using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class CamductJobBuilder
    {
        public CamductJob BuildCamductJob(string jobName, RawFileDataDictionary jobFileDataDictionary)
        {
            FileTypeO oFile = null;
            FileTypeA aFile = null;
            FileTypeS sFile = null;

            foreach (var file in jobFileDataDictionary)
            {
                var filePath = file.Key;
                var fileType = filePath.Substring(filePath.Length - 6);
                var fileData = file.Value;

                switch (fileType)
                {
                    case ".A.TXT":
                        aFile = BuildAFile(filePath, fileData);
                        break;
                    case ".S.TXT":
                        sFile = BuildSFile(filePath, fileData);
                        break;
                    default:
                        oFile = BuildOFile(filePath, fileData);
                        break;
                }
            }

            return new CamductJob(jobName, oFile.First().PlantField.GetValue(), oFile, aFile, sFile);
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

        FileTypeA BuildAFile(string filePath, IEnumerable<string[]> fileData)
        {
            try
            {
                var aFile = new FileTypeA(filePath);
                foreach (var line in fileData)
                {
                    aFile.Add(new TypeAData(line));
                }
                return aFile;
            }
            catch (Exception ex)
            {
                var aFile = new FileTypeA(filePath);
                aFile.RegisterNewException(ex);
                return aFile;
            }
        }

        FileTypeS BuildSFile(string filePath, IEnumerable<string[]> fileData)
        {
            try
            {
                var sFile = new FileTypeS(filePath);
                foreach (var line in fileData)
                {
                    sFile.Add(new TypeSData(line));
                }
                return sFile;
            }
            catch (Exception ex)
            {
                var sFile = new FileTypeS(filePath);
                sFile.RegisterNewException(ex);
                return sFile;
            }
        }
    }
}
