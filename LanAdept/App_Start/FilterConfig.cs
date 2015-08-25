using System.Web;
using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdept
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());

			#if DEBUG
				filters.Add(new AuthorizationRequiredAttribute());
			#endif
		}
	}
}
