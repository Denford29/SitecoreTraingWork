using System.Web.Mvc;

namespace TAC.Utils.Helpers
{
    public class ValidateFormHandler : ActionMethodSelectorAttribute
    {
        public static readonly string ControllerTag = "_controller";
        public static readonly string ActionTag = "_action";

        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            var scController = string.Concat(controllerContext.HttpContext.Request.Form[ControllerTag], "Controller");
            if (scController != methodInfo.DeclaringType.Name) return false;
            return controllerContext.HttpContext.Request.Form[ActionTag] == methodInfo.Name;
        }
    }
}
