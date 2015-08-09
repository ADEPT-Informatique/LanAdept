using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdept.Controllers
{
    [AllowAnonymous]
	public class HomeController : Controller
	{
		UnitOfWork uow = new UnitOfWork();

		[Permission("LanAdept.Home.Index")]
		public ActionResult Index()
		{
            ViewBag.Role = uow.RoleRepository.Get().First().Name;
			return View();
		}
	}
}