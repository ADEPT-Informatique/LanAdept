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
			if (HasActionPermissionExtension.HasActionPermission(helper, actionName, controllerName))
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
			if (HasActionPermissionExtension.HasActionPermission(helper, actionName, controllerName))
				return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);

			if (showText)
				return new MvcHtmlString(linkText);
			else
				return MvcHtmlString.Empty;
		}

	}
}
