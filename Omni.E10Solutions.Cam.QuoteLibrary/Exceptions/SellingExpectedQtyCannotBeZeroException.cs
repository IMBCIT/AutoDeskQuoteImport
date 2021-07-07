using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    class SellingExpectedQtyCannotBeZeroException : Exception
    {
        public SellingExpectedQtyCannotBeZeroException()
            : base(string.Format("Selling Expected Qty is zero. It cannot be zero. This will lead to a divide by zero error. Check to make sure labor is not grouping by itself on galv or nongalv rollup groups. If not, then check the quantities are not zero for each rollup group."))
        {
        }
    }
}
