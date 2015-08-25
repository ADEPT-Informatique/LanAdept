using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LanAdeptCore.MVCModification
{
	public class ViewEngine : RazorViewEngine
	{

		private static string[] NewPartialViewFormats = new[] {
			"~/Views/{1}/Partial/{0}.cshtml",
			"~/Views/Shared/Partial/{0}.cshtml"
		};

		public ViewEngine()
		{
			base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
		}

	}
}
