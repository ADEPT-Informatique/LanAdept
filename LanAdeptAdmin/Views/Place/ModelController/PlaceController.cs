using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.Places.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using PagedList;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptData.Model.Maps;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Users;

namespace LanAdeptAdmin.Controllers
{
	public class PlaceController : Controller
	{
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";
		private const string ERROR_PLACE_OCCUPIED = "Cette place est présentement occupé. Vous devez libérer la place avant de pouvoir modifier la réservation";
		private const string WARNING_PLACE_OCCUPIED = "Attention! Cette place est déjà réservée. Si vous changer le nom de la personne dans ce formulaire, vous annulerez du même coup la réservation de cette personne.";

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();

			listeModel.Maps = uow.MapRepository.Get();

			return View(listeModel);
		}

		[LanAuthorize(Roles = "placeAdmin")]
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

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Search(SearchModel model)
		{
			if (ModelState.IsValid && model.Query != null)
			{
				Reservation byBarcode = uow.ReservationRepository.GetByBarCode(model.Query);
				if (byBarcode != null)
				{
					List<Reservation> reservations = new List<Reservation>();
					reservations.Add(byBarcode);

					model.ReservationsFound = reservations;
				}
				else
				{
					model.ReservationsFound = uow.ReservationRepository.SearchByNameAndEmail(model.Query);
				}


				if (model.ReservationsFound.Count() == 0)
				{
					TempData["Error"] = "Aucune réservation n'as été trouvé pour \"" + model.Query + "\"";
				}
				else if (model.ReservationsFound.Count() == 1)
				{
					Reservation reservationFound = model.ReservationsFound.First();

					return RedirectToAction("Details", new { id = reservationFound.Place.PlaceID });
				}
				else
				{
					model.ReservationsFound = model.ReservationsFound.OrderBy(y => y.UserCompleteName);
				}
			}

			return View(model);
		}

		[LanAuthorize(Roles = "placeAdmin")]
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

			IEnumerable<User> Userlist = uow.UserRepository.Get().OrderBy(u => u.CompleteName);

			ReserveModel model = new ReserveModel();

			if (!placeAReserver.IsFree)
			{
				TempData["Warning"] = WARNING_PLACE_OCCUPIED;

				if (placeAReserver.LastReservation.IsGuest)
				{
					model.FullNameNoAccount = placeAReserver.LastReservation.Guest.CompleteName;
					model.IsGuest = true;
				}
				else
				{
					model.UserID = placeAReserver.LastReservation.User.Id;
				}
			}

			model.Users = new SelectList(Userlist, "Id", "CompleteName");
			model.PlaceID = placeAReserver.PlaceID;
			model.Place = placeAReserver;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]  //Le code de cette action est affreux
		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Reserve([Bind(Include = "PlaceID,UserID,IsGuest,Place,FullNameNoAccount")] ReserveModel model)
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
				if (!model.IsGuest)     //Enregistrement pour un User inscrit
				{
					if (!currentPlace.IsFree && currentPlace.LastReservation.User.Id == model.UserID)
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
							{
								TempData["Success"] = "La place a bien été enregistré";
								return RedirectToAction("Details", new { id = currentPlace.PlaceID });
							}
						}
					}
				}
				else                //Enregistrement pour un invité
				{
					if (!currentPlace.IsFree)
						ReservationService.CancelReservation(currentPlace);

					BaseResult result = ReservationService.ReservePlace(currentPlace, model.FullNameNoAccount);

					if (result.HasError)
						ModelState.AddModelError("", result.Message);

					else
					{
						TempData["Success"] = "La place a bien été enregistré";
						return RedirectToAction("Details", new { id = currentPlace.PlaceID });
					}
				}
			}

			model.PlaceID = currentPlace.PlaceID;
			model.Place = currentPlace;
			ViewBag.UserID = new SelectList(uow.UserRepository.Get().OrderBy(u => u.CompleteName), "UserID", "CompleteName");

			return View(model);
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Cancel(int? id)
		{
			if (id == null || id < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}
			Place currentPlace = uow.PlaceRepository.GetByID(id.Value);
			if (currentPlace == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			if (currentPlace.IsFree)
			{
				TempData["Error"] = "Impossible d'annuler la réservation de cette place car celle-ci n'est pas réservée.";
			}
			else if (currentPlace.LastReservation.HasArrived)
			{
				TempData["Error"] = "Impossible d'annuler la réservation car la personne qui a réservée cette place est déjà arrivée.";
			}
			else
			{
				ReservationService.CancelReservation(currentPlace);

				TempData["Success"] = "Cette place n'est plus réservée.";
			}

			return RedirectToAction("Details", new { id = currentPlace.PlaceID });
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Arriving(int? id)
		{
			if (id == null || id < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}
			Place currentPlace = uow.PlaceRepository.GetByID(id.Value);
			if (currentPlace == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			if (currentPlace.IsFree)
			{
				TempData["Error"] = "Impossible de marquer comme quoi cette place est occupée car celle-ci n'a pas de réservation.";
			}
			else if (currentPlace.LastReservation.HasArrived)
			{
				TempData["Error"] = "Impossible de marquer comme quoi cette place est occupée car elle est déjà occupée.";
			}
			else
			{
				currentPlace.LastReservation.ArrivalDate = DateTime.Now;
				uow.ReservationRepository.Update(currentPlace.LastReservation);
				uow.Save();

				TempData["Success"] = "Cette place est maintenant occupée.";
			}

			return RedirectToAction("Details", new { id = currentPlace.PlaceID });
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Leaving(int? id)
		{
			if (id == null || id < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}
			Place currentPlace = uow.PlaceRepository.GetByID(id.Value);
			if (currentPlace == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			if (currentPlace.IsFree)
			{
				TempData["Error"] = "Impossible de marquer comme quoi cette place est maintenant disponible car celle-ci l'est déjà.";
			}
			else if (!currentPlace.LastReservation.HasArrived)
			{
				TempData["Error"] = "Impossible de marquer comme quoi la personne à cette place est partie puisqu'elle n'est pas encore arrivée.";
			}
			else
			{
				currentPlace.LastReservation.LeavingDate = DateTime.Now;
				uow.ReservationRepository.Update(currentPlace.LastReservation);
				uow.Save();

				TempData["Success"] = "Cette place est maintenant libérée.";
			}

			return RedirectToAction("Details", new { id = currentPlace.PlaceID });
		}

#if DEBUG

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult Reset()
		{
			return View();
		}

		[LanAuthorize(Roles = "placeAdmin")]
		public ActionResult DoReset()
		{
			IEnumerable<Reservation> reservations = uow.ReservationRepository.Get();
			foreach (Reservation reservation in reservations)
			{
				uow.ReservationRepository.Delete(reservation);
			}

			IEnumerable<Tile> tiles = uow.TileRepository.Get();
			foreach (Tile tile in tiles)
			{
				uow.TileRepository.Delete(tile);
			}

			IEnumerable<Place> places = uow.PlaceRepository.Get();
			foreach (Place place in places)
			{
				uow.PlaceRepository.Delete(place);
			}

			IEnumerable<PlaceSection> sections = uow.PlaceSectionRepository.Get();
			foreach (PlaceSection section in sections)
			{
				uow.PlaceSectionRepository.Delete(section);
			}

			IEnumerable<Map> maps = uow.MapRepository.Get();
			foreach (Map map in maps)
			{
				uow.MapRepository.Delete(map);
			}

			Map newMap = new Map();
			newMap.MapName = "Caféteria orange";
			newMap.Width = 18;
			newMap.Height = 13;
			newMap.Tiles = new List<Tile>();

			int x = 1;
			int y = 0;
			for (char sectionName = 'A'; sectionName <= 'H'; sectionName++)
			{
				PlaceSection section = new PlaceSection();
				section.Name = sectionName.ToString();

				for (int i = 1; i <= 24; i++)
				{
					if (i == 13)
					{
						x++;
						y = 0;
					}
					y++;
					Place place = new Place();
					place.Number = i;
					place.PlaceSection = section;

					Tile tile = new Tile();

					tile.Place = place;
					tile.PositionX = x;
					tile.PositionY = y;

					newMap.Tiles.Add(tile);

					uow.PlaceRepository.Insert(place);
					uow.TileRepository.Insert(tile);
				}
				x++;
				y = 0;

				uow.PlaceSectionRepository.Insert(section);
			}

			x = 0;
			for (char sectionName = 'I'; sectionName <= 'J'; sectionName++)
			{
				PlaceSection section = new PlaceSection();
				section.Name = sectionName.ToString();

				for (int i = 1; i <= 9; i++)
				{
					Place place = new Place();
					place.Number = i;
					place.PlaceSection = section;

					Tile tile = new Tile();

					tile.Place = place;
					tile.PositionX = x;
					tile.PositionY = 0;

					newMap.Tiles.Add(tile);

					x++;

					uow.PlaceRepository.Insert(place);
				}
				x = 9;

				uow.PlaceSectionRepository.Insert(section);
			}

			uow.MapRepository.Insert(newMap);

			uow.Save();

			return RedirectToAction("Index");
		}

#endif

	}
}