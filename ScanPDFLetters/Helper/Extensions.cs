using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScanPDFLetters.Helper
{
    public static class Extensions
    {
        public static decimal TryParseDecimal(this string value)
        {
            decimal result;

            var s = Regex.Replace(value, "[^0-9.-]", "");

            decimal.TryParse(s, out result);
            return result;
        }
    }
}
