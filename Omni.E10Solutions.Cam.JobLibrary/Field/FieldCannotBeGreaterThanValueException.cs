using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class FieldCannotBeGreaterThanValueException : Exception
    {
        public FieldCannotBeGreaterThanValueException(int position, string name, string value, decimal max)
             : base(string.Format("The value '{0}' in field '{1}' (position {2}) cannot be greater than {3}.", value, name, position, max))
        {

        }
    }
}
