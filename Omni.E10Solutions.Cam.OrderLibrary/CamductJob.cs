using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.OrderLibrary
{
    class CamductJob
    {
        public readonly string Name;
        public readonly string Plant;
        public FileTypeO OFile { get; protected set; }

        private List<Exception> _exceptions = new List<Exception>();

        string _customerId;
        List<string> _partnumbers;

        public CamductJob(string name, string plant, FileTypeO oFile)
        {
            Name = name;
            Plant = plant;
            OFile = oFile;

            // at construction, cache the customerid and part numbers.
            if (!(OFile is CorruptedFileTypeO))
            {
                _customerId = OFile.First().CustomerId;
                _partnumbers = OFile.Select(f => f.ItemAlias).Distinct().ToList();
            }

            // store corrupted file exceptions
            if (OFile is CorruptedFileTypeO)
                this.RegisterException((OFile as CorruptedFileTypeO).Exception);
        }

        public void RegisterException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        public ReadOnlyCollection<Exception> GetRegisteredExceptions()
        {
            return new ReadOnlyCollection<Exception>(_exceptions);
        }

        public string GetCustomerId()
        {
            return _customerId ?? "";
        }

        public ReadOnlyCollection<string> GetPartNumbers()
        {
            return new ReadOnlyCollection<string>(_partnumbers);
        }

        public string[] GetFilePaths()
        {
            return new string[] { OFile?.Path };
        }

        public bool IsCorrupted()
        {
            return OFile is CorruptedFileTypeO;
        }

        public bool IsInvalid()
        {
            return _exceptions.Count > 0;
        }
    }
}
