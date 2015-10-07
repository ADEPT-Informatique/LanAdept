using System;
using System.Linq;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdept.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		UnitOfWork uow = new UnitOfWork();

		public ActionResult Index()
		{
			ViewBag.PlacesReservee = uow.PlaceRepository.Get().Count(x => !x.IsFree);

			Setting settings = uow.SettingRepository.GetCurrentSettings();
			DateTime dateLan = TimeZoneInfo.ConvertTimeToUtc(settings.StartDate);

            Setting setting = uow.SettingRepository.GetCurrentSettings();
            ViewBag.Description = setting.Description;

			double dateLanMs = dateLan.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; //Donne le temps en unix time

			ViewBag.dateLan = dateLanMs;
			return View();
		}

        public ActionResult About() {
            return View();
        }
	}
}