using System.Web.Mvc;

namespace LanAdeptAdmin.Controllers
{
	public class HomeController : Controller
	{
		[Authorize]
		public ActionResult Index()
		{
			return View();
		}
	}
}