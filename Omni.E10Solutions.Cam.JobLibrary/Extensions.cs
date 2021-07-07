using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omni.E10Solutions.Cam.JobLibrary
{
    static class Extensions
    {
        public static string StripQuotationMarks(this string source)
        {
            return source.Replace("\"", "");
        }

        public static decimal AsDecimal(this string source)
        {
            // [?] does Omni want invalid decimals to be thrown as exceptions or treated as 0?
            decimal d = 0m;
            decimal.TryParse(source, out d);
            return d;
        }

        public static int AsInt(this string source)
        {
            int i = 0;
            int.TryParse(source, out i);
            return i;
        }

        public static DateTime GetNextBusinesDay(this DateTime source)
        {
            switch(source.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return source.AddDays(3).Date;
                case DayOfWeek.Saturday:
                    return source.AddDays(2).Date;
                default:
                    return source.AddDays(1).Date;
            }
        }
    }
}
