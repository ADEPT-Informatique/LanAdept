using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LanAdeptCore.Attribute.Authorization
{
	/// <summary>
	/// Use this attribute to redirect the user if he is logged in 
	/// </summary>
	public class AuthorizeGuestOnlyAttribute : AuthorizeAttribute 
	{
		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			return !httpContext.User.Identity.IsAuthenticated;

		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new RedirectResult("~/Home/Index");
		}
	}
}
