using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HDNHD.Core.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SecuredController : Controller
    {

        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {

            base.OnAuthentication(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}