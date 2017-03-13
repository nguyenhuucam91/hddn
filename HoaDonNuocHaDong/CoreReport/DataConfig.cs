using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HvitFramework.CoreData
{
    public class DataConfig
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ReportConString"].ConnectionString;
          
	public static string RedirectController = "account";
        public static string RedirectAction = "login";
        public static string TechnicalUser = "administrator";
    }
}