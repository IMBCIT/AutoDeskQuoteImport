using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class RawFileDataDictionary : Dictionary<string, IEnumerable<string[]>>
    {
        public RawFileDataDictionary() : base() { }

        public RawFileDataDictionary(IEnumerable<KeyValuePair<string, IEnumerable<string[]>>> data)
            : base(data.ToDictionary(d=>d.Key, d=>d.Value)) { }
    }
}
