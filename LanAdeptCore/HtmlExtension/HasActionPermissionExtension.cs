using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LanAdeptCore.HtmlExtension
{
	public static class HasActionPermissionExtension
	{
		/// <summary>
		/// Determine if the current user has permission to access an action
		/// </summary>
		/// <param name="actionName">Action to test</param>
		/// <returns>True if the user has access, or else false</returns>
		public static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName)
		{
			return HasActionPermission(htmlHelper, actionName, null);
		}

		/// <summary>
		/// Determine if the current user has permission to access an action
		/// </summary>
		/// <param name="actionName">Action to test</param>
		/// <param name="controllerName">Controller to test</param>
		/// <returns>True if the user has access, or else false</returns>
		public static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName)
		{
			ControllerBase controllerToLinkTo = null;

			if (string.IsNullOrEmpty(controllerName))
			{
				controllerToLinkTo = htmlHelper.ViewContext.Controller;
			}
			else
			{
				controllerToLinkTo = GetControllerByName(htmlHelper, controllerName);
			}

			ControllerContext controllerContext = new ControllerContext(htmlHelper.ViewContext.RequestContext, controllerToLinkTo);

			ReflectedControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerToLinkTo.GetType());
			ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

			return ActionIsAuthorized(controllerContext, actionDescriptor);
		}

		private static bool ActionIsAuthorized(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		{
			if (actionDescriptor == null)
				return false;

			AuthorizationContext authContext = new AuthorizationContext(controllerContext, actionDescriptor);

			object[] controllerAttributes = authContext.Controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);

			foreach (AuthorizeAttribute attr in controllerAttributes)
			{
				attr.OnAuthorization(authContext);

				if (authContext.Result != null)
					return false;
			}

			foreach (Filter authFilter in FilterProviders.Providers.GetFilters(authContext, actionDescriptor))
			{
				if (authFilter.Instance is System.Web.Mvc.AuthorizeAttribute)
				{
					((IAuthorizationFilter)authFilter.Instance).OnAuthorization(authContext);

					if (authContext.Result != null)
						return false;
				}
			}

			return true;
		}

		private static ControllerBase GetControllerByName(HtmlHelper helper, string controllerName)
		{
			var tempRequestContext = new RequestContext(helper.ViewContext.RequestContext.HttpContext, new RouteData());

			IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
			IController controller = null;

			try
			{
				controller = factory.CreateController(tempRequestContext, controllerName);
			}
			catch (HttpException)
			{
				throw new ArgumentException("Le controlleur \"" + controllerName + "\" n'est pas trouvable. Vérifiez que les controlleur spécifié dans les AuthorizeActionLink et HtmlHelper similaires sont correctement écrit.");
			}

			if (controller == null)
			{
				throw new InvalidOperationException(
					string.Format(
						CultureInfo.CurrentUICulture,
						"Controller factory {0} controller {1} returned null",
						factory.GetType(),
						controllerName));
			}

			return (ControllerBase)controller;
		}
	}
}
