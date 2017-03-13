using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.ThuNgan
{
    public class ThuNganAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ThuNgan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ThuNgan_default",
                "ThuNgan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}