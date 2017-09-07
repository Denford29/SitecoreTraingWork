using System.Web.Mvc;

namespace events.tac.local.Areas.ATDW
{
    /// <inheritdoc />
    /// <summary>
    /// register the atdw area to the site areas
    /// </summary>
    public class AtdwAreaRegistration : AreaRegistration 
    {
        /// <inheritdoc />
        /// <summary>
        /// define the area name
        /// </summary>
        public override string AreaName => "ATDW";

        /// <inheritdoc />
        /// <summary>
        /// register the default route for the area
        /// </summary>
        /// <param name="context"></param>
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