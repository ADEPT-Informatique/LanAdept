using System;
using System.Linq;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using LanAdeptData.Model.Settings;

namespace LanAdept.Controllers
{
	[AllowAnonymous]
	public class HomeController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		public ActionResult Index()
		{
			ViewBag.PlacesReservee = uow.PlaceRepository.Get().Count(x => !x.IsFree);
			ViewBag.PlacesTotal = uow.PlaceRepository.Get().Count();

			Setting setting = uow.SettingRepository.GetCurrentSettings();

			ViewBag.Description = setting.Description;

			DateTime dateLan = TimeZoneInfo.ConvertTimeToUtc(setting.StartDate);
			double dateLanMs = dateLan.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; //Donne le temps en unix time
			DateTime dateLanEnd = TimeZoneInfo.ConvertTimeToUtc(setting.EndDate);
			double dateLanEndMs = dateLanEnd.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

			ViewBag.dateLan = dateLanMs;
			ViewBag.dateLanEnd = dateLanEndMs;
			ViewBag.dateLanHuman = setting.StartDate;
            ViewBag.isOver = uow.SettingRepository.GetCurrentSettings().IsLanOver;
            ViewBag.isStarted = uow.SettingRepository.GetCurrentSettings().IsLanStarted;
            ViewBag.inscription = uow.SettingRepository.GetCurrentSettings().IsPlaceReservationStarted;
            return View();
		}

		public ActionResult About()
		{
			ViewBag.Rules = uow.SettingRepository.GetCurrentSettings().Rules;
			return View();
		}
	}
}