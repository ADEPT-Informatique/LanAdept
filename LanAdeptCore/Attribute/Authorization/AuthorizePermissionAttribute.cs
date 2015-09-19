using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptCore.Attribute.Authorization
{
	/// <summary>
	/// Use this attribute to specify the permission required to access an action
	/// </summary>
	public class AuthorizePermissionAttribute : AuthorizeAttribute
	{
		private string _permissionName;

		/// <summary>
		/// Specify the permission required to access this action or whole controller
		/// </summary>
		/// <param name="permissionName">Permission name in the database</param>
		public AuthorizePermissionAttribute(string permissionName)
		{
			_permissionName = permissionName;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			Permission permissionToCheck = UnitOfWork.Current.PermissionRepository.GetPermissionByName(_permissionName);

			if (permissionToCheck == null)
				throw new NullReferenceException("La permission \"" + _permissionName + "\" n'existe pas dans la base de donnée sur l'action \"" + filterContext.Controller + "." + filterContext.ActionDescriptor.ActionName + "\"");

			if (!HttpContext.Current.User.Identity.IsAuthenticated)
			{
				if (permissionToCheck.MinimumRoleLevel > 0)
				{ 
					// If is a guest and is not permitted
					UrlHelper url = new UrlHelper(filterContext.RequestContext);
					string signInUrl = url.Action("Login", "Auth", new { ReturnUrl = filterContext.HttpContext.Request.UrlReferrer });
					filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
				}
				return;
			}

			User user = UnitOfWork.Current.UserRepository.GetUserByEmail(HttpContext.Current.User.Identity.Name);
			if (user == null || permissionToCheck.MinimumRoleLevel > user.Role.PermissionLevel)
			{ 
				// If user is not found or user does not have permission
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You do not have access to this page!");
			}
		}
	}
}
