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
using LanAdeptData.DAL;
using System.Linq;
using System.Security.Claims;
using LanAdept.Emails;
using LanAdeptData.Model.Users;
using System.Security.Cryptography;

namespace LanAdept.Controllers
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

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
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

			var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					var user = await UserManager.FindByEmailAsync(model.Email);

					if (!await UserManager.IsEmailConfirmedAsync(user.Id))
						return RedirectToAction("UnconfirmedLogout", new { returnUrl = returnUrl });

					return RedirectToReturnUrl(returnUrl);

				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Tentative de connexion non valide.");
					return View(model);
			}
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
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

			// Connecter cet utilisateur à ce fournisseur de connexion externe si l'utilisateur possède déjà une connexion
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToReturnUrl(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// Si l'utilisateur n'a pas de compte, invitez alors celui-ci à créer un compte
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

					ViewBag.Rules = uow.SettingRepository.GetCurrentSettings().Rules;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, CompleteName = loginInfo.ExternalIdentity.Name });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			if (ModelState.IsValid)
			{
				// Obtenez des informations sur l’utilisateur auprès du fournisseur de connexions externe
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}

				var user = new User {
					UserName = model.Email,
					Email = model.Email,
					CompleteName = model.CompleteName,
					Barcode = GetNewBarcode()
				};

				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToReturnUrl(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		[LanAuthorize]
		public ActionResult Logout()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[LanAuthorize]
		public ActionResult UnconfirmedLogout(string returnUrl)
		{
			TempData["Error"] = true;
			AuthenticationManager.SignOut();
			return RedirectToAction("Login", new { returnUrl = returnUrl });
		}

		[AuthorizeGuestOnly]
		public ActionResult Register()
		{
			ViewBag.Rules = uow.SettingRepository.GetCurrentSettings().Rules;
			return View();
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new User
				{
					UserName = model.Email,
					Email = model.Email,
					CompleteName = model.CompleteName,
					Barcode = GetNewBarcode()
				};

				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					// Send confirmation link by email
					string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					ConfirmationEmail email = new ConfirmationEmail();
					email.User = user;
					email.ConfirmationToken = code;
					email.Send();

					var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");

					MessageModel messageView = new MessageModel()
					{
						Title = "Vous êtes maintenant inscrit!",
						Content = "Vous devez maintenant <strong>confirmer votre email</strong>. Une fois que ce sera fait, vous pourrez réserver une place pour participer au LAN de l'ADEPT.",
						Type = AuthMessageType.Success
					};

					return View("Message", messageView);
				}
				AddErrors(result);
			}

			model.Password = string.Empty;
			model.PasswordConfirmation = string.Empty;

			ViewBag.Rules = uow.SettingRepository.GetCurrentSettings().Rules;
			return View(model);
		}

		[AuthorizeGuestOnly]
		public async Task<ActionResult> Confirm(string id, string code)
		{
			User user = uow.UserRepository.GetByID(id);

			try {
				var result = await UserManager.ConfirmEmailAsync(id, code);

				if (result.Succeeded)
				{
					return View("Message", new MessageModel() { Title = "Félicitation, " + user.CompleteName, Content = "Votre compte est activé! Vous pouvez maintenant vous connecter et réserver une place.", Type = AuthMessageType.Success });
				}
			}
			catch(InvalidOperationException) { }
			return View("Message", new MessageModel() { Title = "Une erreur est survenue", Content = "Ce lien n'est pas valide. Si vous continuez de voir cette erreur, contactez un administrateur.", Type = AuthMessageType.Error });
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

		private string GetNewBarcode()
		{
			RandomNumberGenerator rng = RandomNumberGenerator.Create();
			Byte[] barcodeBytes = new Byte[6];
			rng.GetBytes(barcodeBytes);

			return BitConverter.ToString(barcodeBytes).Replace("-", "");
		}
	}
}