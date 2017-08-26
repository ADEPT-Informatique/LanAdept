using System.Web.Mvc;

namespace LanAdept.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
		public ActionResult Index()
		{
			Response.StatusCode = 500;
			return View();
		}

        public ActionResult PageNotFound()
        {
			Response.StatusCode = 404;
            return View();
        }

		public ActionResult Forbidden()
		{
			Response.StatusCode = 403;
			return View();
		}
    }
}