using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace HDNHD.Core.Helpers
{
    public class StringHelpers
    {
        public static string TrimEllipsis(string source, int length)
        {
            return source.Length <= length ? source : source.Substring(0, length) + " ...";
        }

        public static string NormalizeTag(string tag)
        {
            return GenerateSlug(tag);
        }

        public static string GenerateSlug(string source)
        {
            var slug = source.ToLower();

            slug = RemoveVietnameseChars(slug);

            slug = Regex.Replace(slug, @"&\w+;", "");
            slug = Regex.Replace(slug, @"[^a-z0-9\-\s]", "");
            slug = slug.Replace(' ', '-');
            slug = Regex.Replace(slug, @"-{2,}", "-");
            slug = slug.TrimStart(new[] { '-' });
            if (slug.Length > 80)
                slug = slug.Substring(0, 79);
            slug = slug.TrimEnd(new[] { '-' });
            return slug;
        }

        public static string GenerateSearchField(string source)
        {
            var result = GenerateSlug(source).Replace('-', ' ');
            if (result.Length > 0 && "0123456789".Contains(result.Substring(0, 1)))
                result = "0" + result;
            return result;
        }

        public static string RemoveVietnameseChars(string source)
        {
            var result = source;
            int index = -1;
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            char[] arrChar = FindText.ToCharArray();
            while ((index = result.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(result[index]);
                result = result.Replace(result[index], ReplText[index2]);
            }
            return result;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetEnumAmbientValue(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            AmbientValueAttribute[] attributes =
                (AmbientValueAttribute[])fi.GetCustomAttributes(
                typeof(AmbientValueAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Value.ToString();
            else
                return value.ToString();
        }
    }
}