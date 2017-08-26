using LanAdept.Models;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Manager;
using LanAdeptData.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
			if (TempData["Error"] is bool && (bool)TempData["Error"])
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
			if (result == SignInStatus.Success)
			{
				var user = await UserManager.FindByEmailAsync(model.Email);

				if (!await UserManager.IsInRoleAsync(user.Id, "admin") && !await UserManager.IsInRoleAsync(user.Id, "owner"))
					return RedirectToAction("SilentLogout");

				return RedirectToReturnUrl(returnUrl);
			}

			ModelState.AddModelError("", "Tentative de connexion non valide.");
			return View(model);
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			ControllerContext.HttpContext.Session.RemoveAll();

			// Demandez une redirection vers le fournisseur de connexions externe
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = returnUrl }));
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			if (result == SignInStatus.Success)
			{
				var user = UserManager.Find(loginInfo.Login);
				if (!await UserManager.IsInRoleAsync(user.Id, "admin") && !await UserManager.IsInRoleAsync(user.Id, "owner") )
					return RedirectToAction("SilentLogout");

				return RedirectToReturnUrl(returnUrl);
			}

			TempData["Error"] = true;
			return RedirectToAction("SilentLogout");
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

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			private const string XsrfKey = "XsrfId";

			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
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