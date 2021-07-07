using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    public interface IField
    {
        int Position { get; }
        string Name { get; }
        string GetTextValue();
        bool IsCorrupted();
        Exception GetCorruptionException();
    }
}
