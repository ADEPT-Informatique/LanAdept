using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Place.ModelController;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdept.Controllers
{
	public class PlaceController : Controller
	{
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";
		private const string ERROR_RESERVE_LAN_STARTED = "Désolé, il est impossible de réserver une place lorsque le LAN est déjà commencé. Vous devrez vous présenter à l'accueil du LAN pour obtenir une place.";
		private const string ERROR_CANCEL_LAN_NO_RESERVATION = "Vous devez avoir une réservation pour pouvoir l'annuler.";
		private const string ERROR_CANCEL_LAN_STARTED = "Désolé, il est impossible d'annler une réservation lorsque le LAN est déjà terminé.";
		private const string ERROR_CANCEL_LAN_OVER = "Désolé, il est impossible d'annuler une réservation lorsque le LAN est déjà terminé.";

		private UnitOfWork uow = UnitOfWork.Current;

		[AllowAnonymous]
		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		[AllowAnonymous]
		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();
			ViewBag.Settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

			listeModel.Sections = uow.PlaceSectionRepository.Get();

			return View(listeModel);
		}

		[Authorize]
		public ActionResult Reserver(int? id)
		{
			if (id == null || id < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			Setting settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

			if (settings.IsLanStarted)
			{
				TempData["Error"] = ERROR_RESERVE_LAN_STARTED;
				return RedirectToAction("Liste");
			}

			Place placeAReserver = uow.PlaceRepository.GetByID(id.Value);

			if (placeAReserver == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			BaseResult result = PlaceService.ReservePlace(placeAReserver);

			if (result.HasError)
			{
				TempData["Error"] = result.Message;
			}
			else {
				TempData["Success"] = "La place <strong>" + placeAReserver + "</strong> a bien été réservée!";
			}

			return RedirectToAction("Liste");
		}

		[Authorize]
		public ActionResult MaPlace()
		{
			if (!PlaceService.HasUserPlace())
			{
				TempData["Error"] = "Vous n'avez pas encore réservé une place.";
				return RedirectToAction("Liste");
			}

			Reservation reservation = UserService.GetLoggedInUser().LastReservation;
			ViewBag.Settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

			return View(reservation);
		}

		[Authorize]
		public ActionResult Annuler()
		{
			if (!PlaceService.HasUserPlace())
			{
				TempData["Error"] = ERROR_CANCEL_LAN_NO_RESERVATION;
				return RedirectToAction("Liste");
			}
			Setting settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

			if (settings.IsLanOver)
			{
				TempData["Error"] = ERROR_CANCEL_LAN_OVER;
				return RedirectToAction("Liste");
			}
			else if (settings.IsLanStarted)
			{
				TempData["Error"] = ERROR_CANCEL_LAN_STARTED;
				return RedirectToAction("Liste");
			}

			PlaceService.CancelUserReservation();

			TempData["Success"] = "Votre réservation a été annulée.";
			return RedirectToAction("Liste");
		}
	}
}