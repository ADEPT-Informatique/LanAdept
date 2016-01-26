using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Auth.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Manager;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LanAdeptData.Model;
using Microsoft.AspNet.Identity;

namespace LanAdeptAdmin.Controllers
{
	public class AuthController : Controller
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

		#endregion

		#region Constructors

		public AuthController()
		{
		}

		public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		#endregion

		[AuthorizeGuestOnly]
		public ActionResult Index()
		{
			return RedirectToAction("Login");
		}

		[AuthorizeGuestOnly]
		public ActionResult Login(string returnUrl)
		{
			if(TempData["Error"] is bool && (bool)TempData["Error"])
			{
				ModelState.AddModelError("", "Tentative de connexion non valide.");
			}

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					var user = await UserManager.FindByEmailAsync(model.Email);

					if (!await UserManager.IsInRoleAsync(user.Id, "admin"))
						return RedirectToAction("SilentLogout");

					return RedirectToReturnUrl(returnUrl);

				case SignInStatus.LockedOut:
					return View("Lockout");
			}
			
			ModelState.AddModelError("", "Tentative de connexion non valide.");
			return View(model);
		}

		[LanAuthorize]
		public ActionResult Logout()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[LanAuthorize]
		public ActionResult SilentLogout(string returnUrl)
		{
			TempData["Error"] = true;
			AuthenticationManager.SignOut();
			return RedirectToAction("Login", new { returnUrl = returnUrl });
		}


		private ActionResult RedirectToReturnUrl(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
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