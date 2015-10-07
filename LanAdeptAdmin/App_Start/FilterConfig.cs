using LanAdeptCore.Attribute.Authorization;
using System.Web;
using System.Web.Mvc;

namespace LanAdeptAdmin
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new AuthenticationVerificationAttribute());

			#if DEBUG
				filters.Add(new AuthorizationRequiredAttribute());
			#endif
		}
	}
}
