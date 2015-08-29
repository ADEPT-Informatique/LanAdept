using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Place.ModelController;
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
		public ActionResult Liste(string message, string type)
		{
			ListeModel listeModel = new ListeModel();

			listeModel.Message = message;
			listeModel.Type = type;
			listeModel.Sections = uow.PlaceSectionRepository.Get();

			return View(listeModel);
		}

		[Authorize]
		public ActionResult Reserver(int? id)
		{
			if (id == null || id < 1)
				return RedirectToAction("Liste", new { message = ERROR_INVALID_ID, type = "erreur" });

			Place placeAReserver = uow.PlaceRepository.GetByID(id.Value);

			if(placeAReserver == null)
				return RedirectToAction("Liste", new { message = ERROR_INVALID_ID, type = "erreur" });

			return RedirectToAction("Liste", new { message = "La place " + placeAReserver.PlaceSection.Name + placeAReserver.Number.ToString("00") + " a été réservée (SIMUL)", type = "succes" });
		}
	}
}