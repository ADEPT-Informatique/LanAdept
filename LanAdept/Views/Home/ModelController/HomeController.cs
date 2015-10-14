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

			Setting setting = uow.SettingRepository.GetCurrentSettings();
			

            ViewBag.Description = setting.Description;

			DateTime dateLan = TimeZoneInfo.ConvertTimeToUtc(setting.StartDate);
			double dateLanMs = dateLan.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; //Donne le temps en unix time
			DateTime dateLanEnd = TimeZoneInfo.ConvertTimeToUtc(setting.EndDate);
			double dateLanEndMs = dateLanEnd.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;


			ViewBag.dateLan = dateLanMs;
			ViewBag.dateLanEnd = dateLanEndMs;
			ViewBag.dateLanHuman = setting.StartDate;
			return View();
		}

        public ActionResult About() {
            return View();
        }
	}
}