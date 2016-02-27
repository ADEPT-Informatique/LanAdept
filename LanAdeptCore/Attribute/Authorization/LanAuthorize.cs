using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LanAdeptCore.Attribute.Authorization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class LanAuthorize : System.Web.Mvc.AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Request.IsAuthenticated)
			{
				if (HttpContext.Current.User.IsInRole("owner"))
				{
					return;
				}

				filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
			}
			else
			{
				base.HandleUnauthorizedRequest(filterContext);
			}
		}
	}
}
