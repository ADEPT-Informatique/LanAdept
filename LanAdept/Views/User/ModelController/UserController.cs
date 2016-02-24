using LanAdept.Views.User.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Service;
using LanAdeptData.DAL;
using LanAdeptData.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanAdept.Controllers 
{
	public class UserController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[LanAuthorize]
		public ActionResult Index()
		{
			User u = UserService.GetLoggedInUser();

			UserModel um = new UserModel();
			um.CompleteName = u.CompleteName;
			um.Email = u.Email;

			return View(um);
		}

		[LanAuthorize]
		public ActionResult Edit()
		{
			User u = UserService.GetLoggedInUser();

			UserModel um = new UserModel();
			um.CompleteName = u.CompleteName;
			um.Email = u.Email;

			return View(um);
		}

		[LanAuthorize]
		[HttpPost]
		public ActionResult Edit(UserModel um)
		{
			if (ModelState.IsValid)
			{
				User u = UserService.GetLoggedInUser();
				u.CompleteName = um.CompleteName;
				u.Email = um.Email;
				u.UserName = um.Email;

				uow.UserRepository.Update(u);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View();
		}


	}
}