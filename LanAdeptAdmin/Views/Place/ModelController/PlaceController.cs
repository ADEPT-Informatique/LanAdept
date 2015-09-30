using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.Places.ModelController;
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
		private const string ERROR_PLACE_OCCUPIED = "Cette place est présentement occupé. Vous devez libérer la place avant de pouvoir modifier la réservation";

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
		public ActionResult Search()
		{
			SearchModel searchModel = new SearchModel();

			return View(searchModel);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Search(SearchModel model)
		{
			if (ModelState.IsValid)
			{
				User byBarcode = uow.UserRepository.GetUserByBarCode(model.Query);
				if (byBarcode != null)
				{
					List<User> users = new List<User>();
					users.Add(byBarcode);

					model.UsersFound = users;
				}
				else
				{
					model.UsersFound = uow.UserRepository.SearchUsersByNameAndEmail(model.Query);
				}


				if (model.UsersFound.Count() == 0)
				{
					TempData["Error"] = "Aucun utilisateur n'as été trouvé";
				}
				else if (model.UsersFound.Count() == 1)
				{
					User userFound = model.UsersFound.First();

					if (!ReservationService.HasUserPlace(userFound))
					{
						TempData["Warning"] = "L'utilisateur n'a pas de réservation";
					}
					else
					{
						return RedirectToAction("Details", new { id = userFound.LastReservation.Place.PlaceID });
					}
				}
				else
				{
					model.UsersFound = model.UsersFound.OrderByDescending(x => ReservationService.HasUserPlace(x))
						.ThenBy(y => y.CompleteName)
						.ThenBy(y => y.Email);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult Reserve(int? id)
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
			if (placeAReserver.IsOccupied)
			{
				TempData["Error"] = ERROR_PLACE_OCCUPIED;
				return RedirectToAction("Details", new { id = id });
			}

			ViewBag.UserID = new SelectList(uow.UserRepository.Get().OrderBy(u => u.CompleteName), "UserID", "CompleteName");
			ReserveModel model = new ReserveModel();
			model.PlaceID = placeAReserver.PlaceID;
			model.Place = placeAReserver;

			return View(model);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]	//Le code de cette action est affreux
		public ActionResult Reserve([Bind(Include = "PlaceID,UserID,IsUser,Place,FullNameNoAccount")] ReserveModel model)
		{
			if (model.PlaceID < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}
			Place currentPlace = uow.PlaceRepository.GetByID(model.PlaceID);
			if (currentPlace == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}
			if (currentPlace.IsOccupied)
			{
				TempData["Error"] = ERROR_PLACE_OCCUPIED;
				return RedirectToAction("Details", new { id = model.PlaceID });
			}

			if (ModelState.IsValid)
			{
				if (model.IsUser)		//Enregistrement pour un User inscrit
				{
					if (model.UserID < 1)
					{
						model.UserID = 0;
						ModelState.AddModelError("", ERROR_INVALID_ID);
					}
					else if (!currentPlace.IsFree || currentPlace.LastReservation.User.UserID == model.UserID)
					{
						TempData["Warning"] = "Aucune donnée n'a été mise à jours";
						return RedirectToAction("Details", new { id = currentPlace.PlaceID });
					}
					else
					{
						User currentUser = uow.UserRepository.GetByID(model.UserID);
						if (currentUser == null)
						{
							ModelState.AddModelError("", ERROR_INVALID_ID);
						}
						else
						{
							if (!currentPlace.IsFree)
								ReservationService.CancelReservation(currentPlace);

							BaseResult result = ReservationService.ReservePlace(currentPlace, currentUser);
							if (result.HasError)
								ModelState.AddModelError("", result.Message);
							else
								TempData["Success"] = "La place a bien été enregistré";
						}
					}
				}
				else
				{

				}
			}

			model.PlaceID = currentPlace.PlaceID;
			model.Place = currentPlace;
			ViewBag.UserID = new SelectList(uow.UserRepository.Get().OrderBy(u => u.CompleteName), "UserID", "CompleteName");

			return View(model);
		}

		//[Authorize]
		//public ActionResult Confirmer(int? id, string placeAction)
		//{
		//	if (id == null || id < 1 || placeAction == null)
		//	{
		//		TempData["Error"] = ERROR_INVALID_ID;
		//		return RedirectToAction("Liste");
		//	}

		//	Place placeAReserver = uow.PlaceRepository.GetByID(id.Value);

		//	if (placeAReserver == null)
		//	{
		//		TempData["Error"] = ERROR_INVALID_ID;
		//		return RedirectToAction("Liste");
		//	}

		//	BaseResult result = null;
		//	switch (placeAction)
		//	{
		//		case "Liberer":
		//			//result = ReservationService.LiberePlaceAdmin(placeAReserver);
		//			break;
		//		case "Reserver":
		//			//result = ReservationService.ReservePlaceAdmin(placeAReserver);
		//			break;
		//		case "Occuper":
		//			result = ReservationService.UserArrived(placeAReserver);
		//			break;
		//	}

		//	if (result == null)
		//	{
		//		TempData["Error"] = "L'action " + placeAction + " n'existe pas.";
		//	}
		//	if (result.HasError)
		//	{
		//		TempData["Error"] = result.Message;
		//	}
		//	else
		//	{
		//		TempData["Success"] = result.Message;
		//	}

		//	return RedirectToAction("Liste");
		//}

		//[Authorize]
		//public ActionResult Reserver(int? id)
		//{
		//	if (id == null || id < 1)
		//	{
		//		TempData["Error"] = ERROR_INVALID_ID;
		//		return RedirectToAction("Liste");
		//	}

		//	Place placeAReserver = uow.PlaceRepository.GetByID(id.Value);

		//	if (placeAReserver == null)
		//	{
		//		TempData["Error"] = ERROR_INVALID_ID;
		//		return RedirectToAction("Liste");
		//	}

		//	BaseResult result = ReservationService.ReservePlace(placeAReserver);

		//	if (result.HasError)
		//	{
		//		TempData["Error"] = result.Message;
		//	}
		//	else
		//	{
		//		TempData["Success"] = "La place <strong>" + placeAReserver + "</strong> a bien été réservée!";
		//	}

		//	return RedirectToAction("Liste");
		//}

	}
}