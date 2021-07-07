using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class FileTypeA : List<TypeAData>
    {
        public string Path { get; protected set; }
        public string FileName { get; protected set; }

        public FileTypeA(string path)
        {
            Path = path;
            FileName = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
}
