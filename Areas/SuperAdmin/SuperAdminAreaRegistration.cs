using System.Web.Mvc;

namespace MvcApplication1.Areas.SuperAdmin
{
    public class SuperAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SuperAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SuperAdmin_default",
                "SuperAdmin/{controller}/{action}/{id}",
                new { controller = "SU", action = "ConfigurationManager", id = UrlParameter.Optional },
                new[] { "MvcApplication1.Areas.SuperAdmin.Controllers" }
            );
        }
    }
}
