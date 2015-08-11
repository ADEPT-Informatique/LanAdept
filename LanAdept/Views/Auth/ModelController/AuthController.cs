using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdept.Controllers
{
    public class AuthController : Controller
    {
		[AuthorizeGuestOnly]
        public ActionResult Index()
        {
			return RedirectToAction("Login");
        }

		[AuthorizeGuestOnly]
		public ActionResult Login(string returnURL)
		{
			return View();
		}

		[AuthorizeGuestOnly]
		public ActionResult Register(string returnURL)
		{
			return View();
		}
    }
}