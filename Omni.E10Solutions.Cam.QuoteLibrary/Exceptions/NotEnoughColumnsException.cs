using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class NotEnoughColumnsException : Exception
    {
        public NotEnoughColumnsException(string fileType)
            : base("The " + fileType + " file does not have enough columns.") { }
    }
}
