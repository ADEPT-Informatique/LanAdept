using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.General.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Controllers
{
	public class GeneralController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[AuthorizePermission("admin.general.settings")]
		public ActionResult Index()
		{
			return RedirectToAction("Settings");
		}

		[AuthorizePermission("admin.general.settings")]
		public ActionResult Settings()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			SettingsModel settingsModel = new SettingsModel(settings); 

			return View(settingsModel);
		}

		[AuthorizePermission("admin.general.settings")]
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

		[AuthorizePermission("admin.general.rules")]
		public ActionResult Rules()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			RulesModel rulesModel = new RulesModel();
			rulesModel.Rules = settings.Rules;

			return View(rulesModel);
		}

		[AuthorizePermission("admin.general.rules")]
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

		[AuthorizePermission("admin.general.description")]
		public ActionResult Description()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			DescriptionModel descriptionModel = new DescriptionModel();
			descriptionModel.Description = settings.Description;

			return View(descriptionModel);
		}

		[AuthorizePermission("admin.general.description")]
		[HttpPost]
		public ActionResult Description(DescriptionModel model)
		{
			if (ModelState.IsValid)
			{
				Setting settings = uow.SettingRepository.GetCurrentSettings();
				settings.Description = model.Description;

				uow.SettingRepository.Update(settings);
				uow.Save();

				TempData["Success"] = "Les changements ont été enregistré";
			}

			return View(model);
		}

		[AuthorizePermission("admin.general.rememberEmail")]
		public ActionResult RememberEmail()
		{
			Setting settings = uow.SettingRepository.GetCurrentSettings();
			RememberEmailModel rememberMailModel = new RememberEmailModel();
			rememberMailModel.RememberEmailContent = settings.RememberEmailContent;

			return View(rememberMailModel);
		}

		[AuthorizePermission("admin.general.rememberEmail")]
		[HttpPost]
		public ActionResult RememberEmail(RememberEmailModel model)
		{
			if (ModelState.IsValid)
			{
				Setting settings = uow.SettingRepository.GetCurrentSettings();
				settings.RememberEmailContent = model.RememberEmailContent;

				uow.SettingRepository.Update(settings);
				uow.Save();

				TempData["Success"] = "Les changements ont été enregistré";
			}

			return View(model);
		}
	}
}