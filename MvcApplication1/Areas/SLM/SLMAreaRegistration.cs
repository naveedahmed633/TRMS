using System.Web.Mvc;

namespace MvcApplication1.Areas.SLM
{
    public class SLMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SLM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SLM_default",
                "SLM/{controller}/{action}/{id}",
                new { controller = "SLM", action = "Dashboard", id = UrlParameter.Optional },
                new[] { "MvcApplication1.Areas.SLM.Controllers" }
            );
        }
    }
}
