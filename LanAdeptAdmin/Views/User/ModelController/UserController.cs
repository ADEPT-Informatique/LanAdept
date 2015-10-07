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

namespace LanAdeptAdmin.Controllers
{
    public class UserController : Controller
    {
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";

		UnitOfWork uow = UnitOfWork.Current;

		[AuthorizePermission("admin.user.index")]
		public ActionResult Index(int? page)
        {
			int currentPage = page ?? 1;

			var users = uow.UserRepository.Get().ToList();
			return View(users.ToPagedList(currentPage, 10));
        }

		[AuthorizePermission("admin.user.details")]
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

		[AuthorizePermission("admin.user.edit")]
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
			model.RoleList = new SelectList(uow.RoleRepository.Get(), "RoleID", "Name", user.RoleID);

			return View(model);
        }

        [HttpPost]
		[AuthorizePermission("admin.user.edit")]
        [ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "UserID,CompleteName,RoleID")] EditModel model)
        {
			User user = uow.UserRepository.GetByID(model.UserID);
			if (user == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Index");
			}
			Role role = uow.RoleRepository.GetByID(model.RoleID);
			if (role == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Details", new { id = user.UserID });
			}

            if (ModelState.IsValid)
            {
				user.CompleteName = model.CompleteName;
				user.Role = role;

				uow.UserRepository.Update(user);
				uow.Save();

				TempData["Success"] = "Les changements ont bien été enregistré";
				return RedirectToAction("Details", new { id = user.UserID });
            }

			ViewBag.RoleID = new SelectList(uow.RoleRepository.Get(), "RoleID", "Name", model.RoleID);
            return View(user);
        }

#if DEBUG
		[AuthorizePermission("admin.user.delete")]
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
		[AuthorizePermission("admin.user.delete")]
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
