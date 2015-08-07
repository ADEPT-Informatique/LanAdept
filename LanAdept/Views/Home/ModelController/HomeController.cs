using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdept.Controllers
{
	public class HomeController : Controller
	{
		UnitOfWork uow = new UnitOfWork();

		public ActionResult Index()
		{
			Random rand = new Random();

			uow.TestRepository.Insert(new Test() { Nom = "test #" + rand.Next(20000) });
			uow.Save();

			ViewBag.Test = uow.TestRepository.Get().Last().Nom;
			return View();
		}
	}
}