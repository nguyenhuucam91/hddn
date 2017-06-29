using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Helpers
{
    public class CurrencyHelpers
    {
        public static string FormatVN(double? value)
        {
            if (value == null) value = 0;

            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            return value.Value.ToString("#,##0", cul.NumberFormat);
        }
    }
}