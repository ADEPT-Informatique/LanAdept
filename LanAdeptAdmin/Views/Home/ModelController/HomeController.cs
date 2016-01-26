using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Controllers
{
	public class HomeController : Controller
	{
		[LanAuthorize]
		public ActionResult Index()
		{
			return View();
		}
	}
}