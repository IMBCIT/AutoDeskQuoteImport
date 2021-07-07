using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class FileTypeS : List<TypeSData>
    {
        public string Path { get; protected set; }
        public string FileName { get; protected set; }

        public FileTypeS(string path)
        {
            Path = path;
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        private List<Exception> _exceptions = new List<Exception>();

        public bool IsCorrupted()
        {
            return _exceptions.Count > 0 || this.Any(s => s.IsCorrupt());
        }

        public void RegisterNewException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        public ReadOnlyCollection<Exception> GetExceptions()
        {
            var lineExceptions = this.SelectMany(s => s.GetExceptions());
            var allExceptions = _exceptions.Concat(lineExceptions).ToList();
            return new ReadOnlyCollection<Exception>(allExceptions);
        }

        public TypeSData GetScrap(string partNumber)
        {
            // find the scrap data matching the part number 
            var sData = this.FirstOrDefault(s =>
            {
                var isInsulation = s.InsulationField.GetTextValue().ToLower() == "yes";
                var scrapPartNumber = isInsulation ? s.MaterialField.GetTextValue() + "-" + s.LinearThicknessField.GetTextValue() : s.MaterialField.GetTextValue() + "-" + s.GaugeField.GetTextValue();
                var isMatching = scrapPartNumber == partNumber;
                return isMatching;
            });

            return sData;
        }
    }
}
