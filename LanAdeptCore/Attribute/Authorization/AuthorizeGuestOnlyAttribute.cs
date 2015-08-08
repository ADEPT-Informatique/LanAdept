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
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				filterContext.Result = new RedirectResult("~/Home/Index");
			}
		}
	}
}
