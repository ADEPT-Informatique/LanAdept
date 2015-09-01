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

			return View(reservation);
		}

		[Authorize]
		public ActionResult Annuler()
		{
			if (!PlaceService.HasUserPlace())
			{
				TempData["Error"] = "Vous devez avoir une réservation pour pouvoir l'annuler.";
				return RedirectToAction("Liste");
			}

			PlaceService.CancelUserReservation();

			TempData["Success"] = "Votre réservation a été annulée.";
			return RedirectToAction("Liste");
		}
	}
}