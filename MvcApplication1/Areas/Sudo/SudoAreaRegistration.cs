using System.Web.Mvc;
namespace MvcApplication1.Areas.Sudo
{
    public class SudoAreaRegistration: AreaRegistration
    {
        public override string AreaName
        {
            get { 
                return "Sudo";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sudo_default",
                "Sudo/{controller}/{action}/{id}",
                new { controller = "admin", action = "defaultPage", id = UrlParameter.Optional },
                new[] { "MvcApplication1.Areas.Sudo.Controllers" }
            );
        }
    }
}