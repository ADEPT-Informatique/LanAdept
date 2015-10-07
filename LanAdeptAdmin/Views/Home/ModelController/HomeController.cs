using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Controllers
{
	public class HomeController : Controller
	{
		[AuthorizePermission("admin.home.index")]
		public ActionResult Index()
		{
			return View();
		}
	}
}