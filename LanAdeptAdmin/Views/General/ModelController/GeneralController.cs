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

		[Authorize]
		public ActionResult Rules()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			RulesModel rulesModel = new RulesModel();
			rulesModel.Rules = settings.Rules;

			return View(rulesModel);
		}

		[Authorize]
		[HttpPost]
		public ActionResult Rules(RulesModel model)
		{
			if (ModelState.IsValid)
			{
				Setting settings = uow.SettingRepository.GetCurrentSettings();
				settings.Rules = model.Rules;

				uow.SettingRepository.Update(settings);
				uow.Save();

				TempData["Success"] = "Les changements ont été enregistré";
			}

			return View(model);
		}

        [Authorize]
        public ActionResult Description() {
            Setting settings = uow.SettingRepository.GetCurrentSettings();
            DescriptionModel descriptionModel = new DescriptionModel();
            descriptionModel.Description = settings.Description;

            return View(descriptionModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Description(DescriptionModel model) {
            if (ModelState.IsValid) {
                Setting settings = uow.SettingRepository.GetCurrentSettings();
                settings.Description = model.Description;

                uow.SettingRepository.Update(settings);
                uow.Save();

                TempData["Success"] = "Les changements ont été enregistré";
            }

            return View(model);
        }
	}
}