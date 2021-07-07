using System;
using System.Collections.Generic;
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
    }
}
