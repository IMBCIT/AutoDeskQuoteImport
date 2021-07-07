using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.QuoteLibrary
{
    public class FieldFailCondition<T>
    {
        public readonly Func<T, bool> HasFailed;
        public readonly Exception FailException;

        public FieldFailCondition(Func<T, bool> hasFailedDelegate, Exception failException)
        {
            HasFailed = hasFailedDelegate;
            FailException = failException;
        }
    }
}
