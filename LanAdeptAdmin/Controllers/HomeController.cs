using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Controllers
{
	public class HomeController : Controller
	{
		[LanAuthorize]
		public ActionResult Index()
		{
			if(Request.IsLocal)
				ViewBag.UrlSite = "http://localhost/";
			else
				ViewBag.UrlSite = "http://lanadept.com/";

			return View();
		}
	}
}