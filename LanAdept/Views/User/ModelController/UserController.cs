using LanAdept.Views.User.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Manager;
using LanAdeptCore.Service;
using LanAdeptData.DAL;
using LanAdeptData.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace LanAdept.Controllers 
{
	public class UserController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		#region Properties

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		#endregion


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

				uow.UserRepository.Update(u);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View();
		}

		[LanAuthorize]
		public ActionResult ChangePassword()
		{
			return View();
		}

		[LanAuthorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
				}

				TempData["Success"] = "Votre mot de passe a bien été changé.";

				return RedirectToAction("Index");
			}
			AddErrors(result);
			return View(model);
		}



		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
	}
}