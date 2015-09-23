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

		public ActionResult Index()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			DateTime dateLan = TimeZoneInfo.ConvertTimeToUtc(settings.StartDate);

			double dateLanMs = dateLan.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; //Donne le temps en unix time

			ViewBag.dateLan = dateLanMs;
			return View();
		}
	}
}