using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace LanAdeptCore.HtmlExtension
{
	public static class AuthorizeActionLinkExtension
	{
		/// <summary>
		/// Create a link if the user has permission, or else does nothing
		/// </summary>
		/// <param name="linkText">Text for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, bool showText = false)
		{
			return AuthorizeActionLink(helper, linkText, actionName, null, null, null, showText);
		}

		/// <summary>
		/// Create a link if the user has permission, or else does nothing
		/// </summary>
		/// <param name="linkText">Text for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, bool showText = false)
		{
			return AuthorizeActionLink(helper, linkText, actionName, controllerName, null, null, showText);
		}

		/// <summary>
		/// Create a link if the user has permission, or else does nothing
		/// </summary>
		/// <param name="linkText">Text for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, bool showText = false)
		{
			return AuthorizeActionLink(helper, linkText, actionName, controllerName, routeValues, null, showText);
		}

		/// <summary>
		/// Create a link if the user has permission, or else does nothing
		/// </summary>
		/// <param name="linkText">Text for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <param name="htmlAttributes">htmlAttributes for the link (for style, classes, ...)</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool showText = false)
		{
			if (HasActionPermission(helper, actionName, controllerName))
				return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);

			if (showText)
				return new MvcHtmlString(linkText);
			else
				return MvcHtmlString.Empty;
		}

		/// <summary>
		/// Create a link if the user has permission, or else does nothing
		/// </summary>
		/// <param name="linkText">Text for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <param name="htmlAttributes">htmlAttributes for the link (for style, classes, ...)</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool showText = false)
		{
			if (HasActionPermission(helper, actionName, controllerName))
				return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);

			if (showText)
				return new MvcHtmlString(linkText);
			else
				return MvcHtmlString.Empty;
		}

		private static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName)
		{
			ControllerBase controllerToLinkTo = null;
			
			if(string.IsNullOrEmpty(controllerName)) {
				controllerToLinkTo = htmlHelper.ViewContext.Controller;
			}
			else {
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

			IController controller = factory.CreateController(tempRequestContext, controllerName);

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
