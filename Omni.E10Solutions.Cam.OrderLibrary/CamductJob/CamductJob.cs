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
            if (!OFile.IsCorrupted())
            {
                _customerId = OFile.First().CustomerIdField.GetValue();
                _partnumbers = OFile.Select(f => f.ItemAliasField.GetValue()).Distinct().ToList();
            }

            // store corrupted file exceptions
            if (OFile.IsCorrupted())
                this.RegisterExceptions(OFile.GetExceptions());
        }

        void RegisterExceptions(IEnumerable<Exception> exes)
        {
            foreach (var ex in exes) RegisterException(ex);
        }

        public void RegisterException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        public ReadOnlyCollection<Exception> GetExceptions()
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
            return OFile.IsCorrupted();
        }

        public bool IsInvalid()
        {
            return _exceptions.Count > 0;
        }
    }
}
