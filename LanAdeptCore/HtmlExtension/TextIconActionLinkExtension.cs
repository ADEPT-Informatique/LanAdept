using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace LanAdeptCore.HtmlExtension
{
	public static class TextIconActionLinkExtension
	{
		/// <summary>
		/// Create a link that contains only an icon if the user has permission, or else does nothing
		/// </summary>
		/// <param name="iconClass">Icon class for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString TextIconActionLink(this HtmlHelper helper, string text, string iconClass, string actionName, bool showText = false)
		{
			return TextIconActionLink(helper, text, iconClass, actionName, null, null, null, showText);
		}

		/// <summary>
		/// Create a link that contains only an icon if the user has permission, or else does nothing
		/// </summary>
		/// <param name="iconClass">Icon class for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString TextIconActionLink(this HtmlHelper helper, string text, string iconClass, string actionName, string controllerName, bool showText = false)
		{
			return TextIconActionLink(helper, text, iconClass, actionName, controllerName, null, null, showText);
		}

		/// <summary>
		/// Create a link that contains only an icon if the user has permission, or else does nothing
		/// </summary>
		/// <param name="iconClass">Icon class for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString TextIconActionLink(this HtmlHelper helper, string text, string iconClass, string actionName, string controllerName, object routeValues, bool showText = false)
		{
			return TextIconActionLink(helper, text, iconClass, actionName, controllerName, routeValues, null, showText);
		}

		/// <summary>
		/// Create a link that contains only an icon if the user has permission, or else does nothing
		/// </summary>
		/// <param name="iconClass">Icon class for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <param name="htmlAttributes">htmlAttributes for the link (for style, classes, ...)</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString TextIconActionLink(this HtmlHelper helper, string text, string iconClass, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool showText = false)
		{
			return TextIconActionLink(helper, text, iconClass, actionName, controllerName, routeValues, htmlAttributes, showText);
		}

		/// <summary>
		/// Create a link that contains only an icon if the user has permission, or else does nothing
		/// </summary>
		/// <param name="iconClass">Icon class for the link</param>
		/// <param name="actionName">Action for the link</param>
		/// <param name="controllerName">Controller for the link</param>
		/// <param name="routeValues">routeValues for the link</param>
		/// <param name="htmlAttributes">htmlAttributes for the link (for style, classes, ...)</param>
		/// <paramparam name="showText">True to show text without a link if unauthorized</paramparam>
		public static MvcHtmlString TextIconActionLink(this HtmlHelper helper, string text, string iconClass, string actionName, string controllerName, object routeValues, object htmlAttributes, bool showText = false)
		{
			if(helper.HasActionPermission(actionName, controllerName))
			{
				RouteValueDictionary routeValuesDict = new RouteValueDictionary(routeValues);
				IDictionary<string, object> htmlAttributesDict = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

				string str = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValuesDict, RouteTable.Routes, HttpContext.Current.Request.RequestContext, true);
				TagBuilder tagBuilder = new TagBuilder("a")
				{
					InnerHtml = "<span class=\"glyphicon " + iconClass + "\"></span> " + MvcHtmlString.Create(text)
				};
				TagBuilder tagBuilder1 = tagBuilder;
				tagBuilder1.MergeAttributes<string, object>(htmlAttributesDict);
				tagBuilder1.MergeAttribute("href", str);
				return MvcHtmlString.Create(tagBuilder1.ToString(TagRenderMode.Normal));
			}
		

			return MvcHtmlString.Empty;
		}
	}
}
