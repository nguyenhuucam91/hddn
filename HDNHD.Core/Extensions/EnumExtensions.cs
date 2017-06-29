using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum e)
        {
            var name = e.ToString();
            var attrs = e.GetType().GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            try
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            } catch (Exception)
            {
                return name;
            }
        }
    }
}