using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using LanAdeptCore.Service;

namespace LanAdeptCore.Attribute.Authorization
{
	public class AuthenticationVerificationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (UserService.IsUserLoggedIn())
			{
				if (UserService.GetLoggedInUser() == null)
				{
					UserService.Logout();
				}
			}
			base.OnActionExecuting(filterContext);
		}
	}
}
