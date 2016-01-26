using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

			//If the user is not logged in, we let him in (He will have to be redirected to the login page anyways)
			IPrincipal user = httpContext.User;
			if (!user.Identity.IsAuthenticated)
			{
				return true;
			}

			if (!base.AuthorizeCore(httpContext))
				return false;

			return httpContext.User.IsInRole("admin");
		}
	}
}
