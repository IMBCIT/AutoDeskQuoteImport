using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public class FileTypeO : List<TypeOData>
    {
        public string Path { get; protected set; }
        public string FileName { get; protected set; }

        private List<Exception> _exceptions = new List<Exception>();

        public FileTypeO(string path)
        {
            Path = path;
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public bool IsCorrupted()
        {
            return _exceptions.Count > 0 || this.Any(o => o.IsCorrupt());
        }

        public void RegisterNewException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        public ReadOnlyCollection<Exception> GetExceptions()
        {
            var lineExceptions = this.SelectMany(o => o.GetExceptions());
            var allExceptions = _exceptions.Concat(lineExceptions).ToList();
            return new ReadOnlyCollection<Exception>(allExceptions);
        }
    }
}
