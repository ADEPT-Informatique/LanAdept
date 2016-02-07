using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LanAdeptCore.Attribute.Authorization
{
	public class AdminOnlyAttribute : LanAuthorize
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			string action = filterContext.ActionDescriptor.ActionName;

			if (!string.IsNullOrWhiteSpace(action) && (action.Trim().ToLower() == "logout" || action.Trim().ToLower() == "silentlogout"))
			{
				return;
			}

			base.OnAuthorization(filterContext);
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}

			if (!base.AuthorizeCore(httpContext))
				return false;

			return httpContext.User.IsInRole("owner") || httpContext.User.IsInRole("admin");
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsAuthenticated)
			{
				filterContext.Result = new RedirectToRouteResult(
								   new RouteValueDictionary
								   {
									   { "action", "silentLogout" },
									   { "controller", "Auth" }
								   });
			}

			//else do normal process
			base.HandleUnauthorizedRequest(filterContext);
		}
	}
}
