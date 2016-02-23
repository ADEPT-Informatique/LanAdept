using LanAdept.Views.User.ModelController;
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

		[AllowAnonymous]
		public ActionResult Index()
		{
			User u = UserService.GetLoggedInUser();

			UserModel um = new UserModel();
			um.CompleteName = u.CompleteName;
			um.Email = u.Email;
			um.UserId = u.Id;

			return View(um);
		}
	}
}