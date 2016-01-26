using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LanAdeptData.DAL;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.MVCModification;

namespace LanAdeptAdmin
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			ViewEngines.Engines.Add(new ViewEngine());
		}

		protected virtual void Application_BeginRequest()
		{
			HttpContext.Current.Items["_UnitOfWork"] = new UnitOfWork();
		}

		protected virtual void Application_EndRequest()
		{
			var uow = HttpContext.Current.Items["_UnitOfWork"] as UnitOfWork;
			if (uow != null)
				uow.Dispose();
		}

	}
}
