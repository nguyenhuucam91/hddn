using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.InHoaDon
{
    public class InHoaDonAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InHoaDon";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InHoaDon_default",
                "InHoaDon/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}