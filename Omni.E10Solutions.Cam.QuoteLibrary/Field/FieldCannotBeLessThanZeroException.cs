using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class FieldCannotBeLessThanZeroException : Exception
    {
        public FieldCannotBeLessThanZeroException(int position, string name, string value)
             : base(string.Format("The value '{0}' in field '{1}' (position {2}) cannot be less than 0.", value, name, position))
        {

        }
    }
}
