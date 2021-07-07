using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class CamductJob
    {
        public readonly string Name;
        public FileTypeO OFile { get; protected set; }
        public FileTypeA AFile { get; protected set; }
        public FileTypeS SFile { get; protected set; }
        public string Plant { get; protected set; }
        public string Company => OFile.FirstOrDefault()?.CompanyField.GetValue();

        private List<Exception> _exceptions = new List<Exception>();

        string _customerId;
        List<string> _nonDtlParts;
        List<string> _dtlOnlyParts;

        public CamductJob(string name, string plant, FileTypeO oFile, FileTypeA aFile, FileTypeS sFile)
        {
            Name = name;
            Plant = plant;
            OFile = oFile;
            AFile = aFile;
            SFile = sFile;

            // at construction, cache the customerid and part numbers.
            if (!OFile.IsCorrupted())
            {
                _customerId = OFile.First().CustomerIdField.GetValue();
                _dtlOnlyParts = OFile.Select(f => f.ItemAliasField.GetValue()).Distinct().ToList();
            }

            // at construction, cache the non dtl part numbers.
            if (!AFile.IsCorrupted() && !OFile.IsCorrupted())
            {
                var aFileParts = AFile.Select(a => a.AncillaryPartNoField.GetTextValue());
                var oFilePartsType1 = OFile.Select(o => o.MaterialField.GetTextValue() + "-" + o.WireGaugeField.GetTextValue());
                var oFilePartsType2 = OFile.Select(o => o.InsulationMaterialField.GetTextValue() + "-" + o.InsulationThicknessField.GetTextValue());
                var oFilePartsType3 = OFile.Select(o => o.DWSkinMaterialField.GetTextValue() + "-" + o.DWSkinGaugeField.GetTextValue());

                var parts = aFileParts.Concat(oFilePartsType1).Concat(oFilePartsType2).Concat(oFilePartsType3).Distinct().ToList();
                _nonDtlParts = parts.Where(p => p != "-").ToList();
            }

            // store corrupted file exceptions
            if (OFile.IsCorrupted())
                this.RegisterExceptions(OFile.GetExceptions());

            if (AFile.IsCorrupted())
                this.RegisterExceptions(AFile.GetExceptions());

            if (SFile.IsCorrupted())
                this.RegisterExceptions(sFile.GetExceptions());
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

        public ReadOnlyCollection<string> GetNonDtlPartNumbers()
        {
            return new ReadOnlyCollection<string>(_nonDtlParts);
        }

        public ReadOnlyCollection<string> GetDtlOnlyPartNumbers()
        {
            return new ReadOnlyCollection<string>(_dtlOnlyParts);
        }

        public ReadOnlyCollection<string> GetAllPartNumbers()
        {
            var allPartNumbers = _dtlOnlyParts.Concat(_nonDtlParts).Distinct().ToList();
            return new ReadOnlyCollection<string>(allPartNumbers);
        }

        public string[] GetFilePaths()
        {
            return new string[] { OFile?.Path, AFile?.Path, SFile?.Path };
        }

        public bool IsCorrupted()
        {
            return OFile.IsCorrupted() || AFile.IsCorrupted() || SFile.IsCorrupted();
        }

        public bool IsInvalid()
        {
            return _exceptions.Count > 0;
        }

        public bool HasBurnOperation()
        {
            return OFile.Any(f => !string.IsNullOrWhiteSpace(f.BURN_Time_minsField.GetTextValue()));
        }

        public Rollup GetRollup(Epicor10Cache cache)
        {
            return new Rollup(OFile, AFile, SFile, HasBurnOperation(), cache);
        }
    }
}
