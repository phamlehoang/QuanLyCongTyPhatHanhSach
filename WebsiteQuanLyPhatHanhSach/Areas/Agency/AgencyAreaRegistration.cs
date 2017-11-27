using System.Web.Mvc;

namespace WebsiteQuanLyPhatHanhSach.Areas.Agency
{
    public class AgencyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Agency";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Agency_default",
                "Agency/{controller}/{action}/{id}",
                new { controller = "Login", action = "Index", id = UrlParameter.Optional },
                new string[] { "WebsiteQuanLyPhatHanhSach.Areas.Agency.Controllers" }
            );
        }
    }
}