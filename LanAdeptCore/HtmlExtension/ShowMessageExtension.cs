using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LanAdeptCore.HtmlExtension
{
	public static class ShowMessageExtension
	{
		/// <summary>
		/// Affiche les messages d'erreur, de succès ou d'avertissement pour la page s'il y en a
		/// </summary>
		public static MvcHtmlString ShowMessage(this HtmlHelper helper)
		{
			var tempData = helper.ViewContext.Controller.TempData;
			StringBuilder resultBuilder = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(tempData["Error"] as string))
			{
				resultBuilder.Append("<div class=\"alert alert-danger\">");
				resultBuilder.Append(tempData["Error"]);
				resultBuilder.AppendLine("</div>");
			}
			if (!string.IsNullOrWhiteSpace(tempData["Success"] as string))
			{
				resultBuilder.Append("<div class=\"alert alert-success\">");
				resultBuilder.Append(tempData["Success"]);
				resultBuilder.AppendLine("</div>");
			}
			if (!string.IsNullOrWhiteSpace(tempData["Warning"] as string))
			{
				resultBuilder.Append("<div class=\"alert alert-warning\">");
				resultBuilder.Append(tempData["Warning"]);
				resultBuilder.AppendLine("</div>");
			}

			return new MvcHtmlString(resultBuilder.ToString());
		}
	}
}
