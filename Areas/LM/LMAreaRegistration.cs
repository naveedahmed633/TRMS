using System.Web.Mvc;

namespace MvcApplication1.Areas.LM
{
    public class LMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "LM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "LM_default",
                "LM/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "MvcApplication1.Areas.LM.Controllers" }
            );
        }
    }
}
