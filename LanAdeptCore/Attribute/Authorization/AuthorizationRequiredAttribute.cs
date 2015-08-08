//This class is used to throw an exception if no authorization attribute are set on an action
//It is used only in debug, for obvious reasons
#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LanAdeptCore.Attribute.Authorization
{
	public class AuthorizationRequiredAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			int nbFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length;
			nbFilter += filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length;

			nbFilter += filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length;
			nbFilter += filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length;

			if (nbFilter == 0)
			{
				string actionName = filterContext.ActionDescriptor.ActionName;
				string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

				throw new InvalidOperationException("The action \"" + controllerName + "." + actionName + "\" does not contain an authorization filter");
			}

			base.OnActionExecuting(filterContext);
		}
	}
}
#endif