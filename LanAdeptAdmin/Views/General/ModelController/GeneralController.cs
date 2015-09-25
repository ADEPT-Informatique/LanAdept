using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.General.ModelController;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Controllers
{
	public class GeneralController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			return RedirectToAction("Settings");
		}

		[Authorize]
		public ActionResult Settings()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			SettingsModel settingsModel = new SettingsModel(settings); 

			return View(settingsModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult Settings(SettingsModel model)
		{
			if (ModelState.IsValid)
			{
				Setting settings = uow.SettingRepository.GetCurrentSettings();
				settings.StartDate = model.StartDate;
				settings.EndDate = model.EndDate;
				settings.SendRememberEmail = model.SendRememberEmail;
				settings.NbDaysBeforeRemember = model.NbDaysBeforeRemember;

				uow.SettingRepository.Update(settings);
				uow.Save();

				TempData["Success"] = "Les changements ont été enregistré";
			}

			return View(model);
		}
	}
}