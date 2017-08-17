using System.Web.Mvc;

namespace events.tac.local.Areas.ATDW
{
    public class ATDWAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ATDW";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ATDW_default",
                "ATDW/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}