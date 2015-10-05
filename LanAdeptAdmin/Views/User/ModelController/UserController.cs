using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using PagedList;

namespace LanAdeptAdmin.Controllers
{
    public class UserController : Controller
    {
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index(int? page)
        {
			int currentPage = page ?? 1;

			var users = uow.UserRepository.Get().ToList();
			return View(users.ToPagedList(currentPage, 10));
        }

		[Authorize]
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

		[Authorize]
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
			ViewBag.RoleID = new SelectList(uow.RoleRepository.Get(), "RoleID", "Name", user.RoleID);
            return View(user);
        }

        [HttpPost]
		[Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Email,Password,Salt,CompleteName,RoleID")] User user)
        {
            if (ModelState.IsValid)
            {
				uow.UserRepository.Update(user);
				uow.Save();
                return RedirectToAction("Index");
            }
			ViewBag.RoleID = new SelectList(uow.RoleRepository.Get(), "RoleID", "Name", user.RoleID);
            return View(user);
        }

		[Authorize]
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
		[Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
			User user = uow.UserRepository.GetByID(id);
			uow.UserRepository.Delete(user);
			uow.Save();

            return RedirectToAction("Index");
        }
    }
}
