using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdeptAdmin.Views.Users.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using PagedList;
using Microsoft.AspNet.Identity.Owin;

namespace LanAdeptAdmin.Controllers
{
	//TODO: Autorisation plus précise
	[LanAuthorize]
	public class UserController : Controller
    {
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		public ActionResult Index(string sortOrder, int? page)
        {
			int currentPage = page ?? 1;

			ViewBag.CurrentSort = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";
			ViewBag.RoleSortParm = sortOrder == "role" ? "role_desc" : "role";
			ViewBag.PlaceSortParm = sortOrder == "place" ? "place_desc" : "place";

			var users = uow.UserRepository.Get();

			switch (sortOrder)
			{
				case "name_desc":
					users = users.OrderByDescending(u => u.CompleteName);
					break;
				case "email":
					users = users.OrderBy(u => u.Email);
					break;
				case "email_desc":
					users = users.OrderByDescending(u => u.Email);
					break;
				case "place":
					users = users.OrderBy(u => (u.LastReservation == null || u.LastReservation.Place.IsFree ? "zz" : u.LastReservation.Place.ToString()))
						.ThenBy(u => u.CompleteName);
					break;
				case "place_desc":
					users = users.OrderByDescending(u => (u.LastReservation == null || u.LastReservation.Place.IsFree ? "zz" : u.LastReservation.Place.ToString()))
						.ThenBy(u => u.CompleteName);
					break;
				default:  // Name ascending 
					users = users.OrderBy(u => u.CompleteName);
					break;
			}

			return View(users.ToPagedList(currentPage, 10));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = uow.UserRepository.GetByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

		public ActionResult Search(SearchModel model)
		{
			if (ModelState.IsValid && model.Query != null)
			{
				model.UsersFound = uow.UserRepository.SearchUsersByNameAndEmail(model.Query);

				if (model.UsersFound.Count() == 0)
				{
					TempData["Error"] = "Aucun utilisateur n'as été trouvé pour \"" + model.Query + "\"";
				}
				else if (model.UsersFound.Count() == 1)
				{
					User userFound = model.UsersFound.First();

					return RedirectToAction("Details", new { id = userFound.Id });
				}
				else
				{
					model.UsersFound = model.UsersFound.OrderBy(y => y.CompleteName);
				}
			}

			return View(model);
		}

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			User user = uow.UserRepository.GetByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }

			EditModel model = new EditModel(user);
			model.RoleList = new SelectList(uow.RoleRepository.Get(), "RoleID", "Name", user.Id);

			return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "UserID,CompleteName,RoleID")] EditModel model)
        {
			User user = uow.UserRepository.GetByID(model.UserID);
			if (user == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Index");
			}
			Role role = uow.RoleRepository.GetByID(model.UserID);
			if (role == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Details", new { id = user.Id });
			}

            if (ModelState.IsValid)
            {
				user.CompleteName = model.CompleteName;

				uow.UserRepository.Update(user);
				uow.Save();

				TempData["Success"] = "Les changements ont bien été enregistré";
				return RedirectToAction("Details", new { id = user.Id });
            }

            return View(user);
        }

#if DEBUG
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			User user = uow.UserRepository.GetByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			User user = uow.UserRepository.GetByID(id);
			uow.UserRepository.Delete(user);
			uow.Save();

            return RedirectToAction("Index");
        }
#endif
    }
}
