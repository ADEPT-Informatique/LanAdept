using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.Place.ModelController;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using PagedList;

namespace LanAdeptAdmin.Controllers
{
	public class PlaceController : Controller
	{
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";

		private UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		[Authorize]
		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();

			listeModel.Sections = uow.PlaceSectionRepository.Get();

			return View(listeModel);
		}


		[Authorize]
		public ActionResult Details(int? id, string sortOrder, string searchString, string currentFilter, int? page)
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

			Place place = uow.PlaceRepository.GetByID(id);

			ViewBag.CurrentSort = sortOrder;
			ViewBag.DateSort = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
			ViewBag.NameSort = sortOrder == "Name" ? "name_desc" : "Name";

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewBag.CurrentFilter = searchString;

			if (!String.IsNullOrEmpty(searchString))
			{
				place.Reservations = place.Reservations.Where(s => s.User.CompleteName.Contains(searchString)).ToList();
			}

			switch (sortOrder)
			{
				case "name_desc":
					place.Reservations = place.Reservations.OrderByDescending(s => s.User.CompleteName).ToList();
					break;
				case "Name":
					place.Reservations = place.Reservations.OrderBy(s => s.User.CompleteName).ToList();
					break;
				case "date_desc":
					place.Reservations = place.Reservations.OrderBy(s => s.CreationDate).ToList();
					break;
				default:
					place.Reservations = place.Reservations.OrderByDescending(s => s.CreationDate).ToList();
					break;
			}

			int pageSize = 5;
			int pageNumber = (page ?? 1);
			ViewBag.Reservations = place.Reservations.ToPagedList(pageNumber, pageSize);
			return View(place);
		}

		[Authorize]
		public ActionResult Confirmer(int? id, string placeAction)
		{
			if (id == null || id < 1 || placeAction == null)
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

			BaseResult result = null;
			switch (placeAction)
			{
				case "Liberer":
					result = PlaceService.LiberePlaceAdmin(placeAReserver);
					break;
				case "Reserver":
					result = PlaceService.ReservePlaceAdmin(placeAReserver);
					break;
				case "Occuper":
					result = PlaceService.OccuperPlaceAdmin(placeAReserver);
					break;
			}

			if (result == null)
			{
				TempData["Error"] = "L'action " + placeAction + " n'existe pas.";
			}
			if (result.HasError)
			{
				TempData["Error"] = result.Message;
			}
			else
			{
				TempData["Success"] = result.Message;
			}

			return RedirectToAction("Liste");
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
			else
			{
				TempData["Success"] = "La place <strong>" + placeAReserver + "</strong> a bien été réservée!";
			}

			return RedirectToAction("Liste");
		}

		[Authorize]
		public ActionResult Search()
		{
			SearchModel searchModel = new SearchModel();
			searchModel.UsersFound = uow.UserRepository.Get()
				.OrderByDescending(x => PlaceService.HasUserPlace(x))
				.ThenBy(y => y.CompleteName)
				.ThenBy(y => y.Email);

			return View(searchModel);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Search(SearchModel model)
		{
			if(ModelState.IsValid)
			{
				IEnumerable<User> usersFound = uow.UserRepository.SearchUsersByNameAndEmail(model.Query);

				if (usersFound.Count() == 0)
				{
					TempData["Error"] = "Aucun utilisateur n'as été trouvé";
				}
				else if (usersFound.Count() == 1)
				{
					User userFound = usersFound.First();

					if(!PlaceService.HasUserPlace(userFound))
					{
						TempData["Warning"] = "L'utilisateur n'a pas de réservation";
					}
					else
					{
						return RedirectToAction("Details", new { id = userFound.LastReservation.Place.PlaceID });
					}
				}

				model.UsersFound = usersFound;
			}

			return View(model);
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