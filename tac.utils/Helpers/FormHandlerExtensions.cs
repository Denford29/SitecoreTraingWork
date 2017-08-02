using Sitecore.Mvc.Presentation;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TAC.Utils.Helpers
{
    public static class FormHandlerExtensions
    {

        public static HtmlString FormHandler(this HtmlHelper html)
        {
            return FormHandler(html, GetRenderingField("controller action"));
        }

        public static HtmlString FormHandler(this HtmlHelper html, string actionName)
        {
            return FormHandler(html, GetRenderingField("controller"), actionName);
        }
        public static HtmlString FormHandler(this HtmlHelper html, string controllerName, string actionName)
        {
            return new HtmlString(
            string.Concat(
                html.Hidden(ValidateFormHandler.ControllerTag, controllerName),
                html.Hidden(ValidateFormHandler.ActionTag, actionName)
                )
            );
        }
        private static string GetRenderingField(string field)
        {
            return RenderingContext.CurrentOrNull.Rendering.RenderingItem.InnerItem[field];
        }
    }
}
