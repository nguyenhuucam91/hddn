using HDNHD.Models.Constants;
using HDNHD.Models.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HDNHD.Core.Models
{
    public class RequestScope
    {
        public static Nguoidung LoggedInUser
        {
            get
            {
                return Get("request_scope_adminuser") as Nguoidung;
            }
            set
            {
                Set("request_scope_adminuser", value);
            }
        }

        public static EUserRole UserRole
        {
            get
            {
                return (EUserRole) Get("request_scope_adminrole");
            }
            set
            {
                Set("request_scope_adminrole", value);
            }
        }

        public static void Set(string key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }
        public static object Get(string key)
        {
            return HttpContext.Current.Items[key];
        }
    }
}