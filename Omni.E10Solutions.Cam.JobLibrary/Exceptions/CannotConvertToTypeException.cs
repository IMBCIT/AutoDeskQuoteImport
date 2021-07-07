using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    class CannotConvertToTypeException : Exception
    {
        public CannotConvertToTypeException(int position, string name, string value, Type type)
            : base(string.Format("The value '{0}' in field {1} (position {2}) cannot be converted to type {3}.", value, name, position, type.ToString()))
        {

        }
    }
}
