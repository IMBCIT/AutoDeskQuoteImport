using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class FileTypeA : List<TypeAData>
    {
        public string Path { get; protected set; }
        public string FileName { get; protected set; }

        private List<Exception> _exceptions = new List<Exception>();

        public FileTypeA(string path)
        {
            Path = path;
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
        }

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
    }
}
