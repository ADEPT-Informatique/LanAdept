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
				var user = new User { UserName = model.Email, Email = model.Email, CompleteName = model.CompleteName };
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
				//TODO: Complété le nouvel user
				var user = new User {
					UserName = model.Email,
					Email = model.Email,
					CompleteName = model.CompleteName
				};

				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// Send confirmation link by email
					string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");

					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}

			model.Password = string.Empty;
			model.PasswordConfirmation = string.Empty;

			ViewBag.Rules = uow.SettingRepository.GetCurrentSettings().Rules;
			return View(model);

			//if (ModelState.IsValid)
			//{
			//	User newUser = UserService.CreateUser(model.Email, model.Password, model.CompleteName);

			//	UnitOfWork.UserRepository.Insert(newUser);
			//	UnitOfWork.Save();

			//	ConfirmationEmail email = new ConfirmationEmail();
			//	email.User = newUser;
			//	email.Send();

			//	MessageModel result = new MessageModel();
			//	result.Title = "Vous êtes maintenant inscrit!";
			//	result.Content = "Vous devez maintenant <strong>confirmer votre email</strong>. Une fois que ce sera fait, vous pourrez réserver une place pour participer au LAN de l'ADEPT.";
			//	result.Type = AuthMessageType.Success;

			//	return View("Message", result);
			//}

			//model.Password = null;
			//model.PasswordConfirmation = null;

			//return View(model);
		}

		[AuthorizeGuestOnly]
		public ActionResult Confirm(string id)
		{
			//User user = UnitOfWork.UserRepository.GetUserByBarCode(id);

			//if (user == null)
			//{
			//	return View("Message", new MessageModel() { Title = "Une erreur est survenue", Content = "Ce lien n'est pas valide. Si vous continuez de voir cette erreur, contactez un administrateur.", Type = AuthMessageType.Error });
			//}

			////if (user.RoleID != UnitOfWork.RoleRepository.GetUnconfirmedRole().RoleID)
			////{
			////	return View("Message", new MessageModel() { Title = "Votre compte est déjà actif", Content = "Vous pouvez maintenant vous connecter et réserver une place.", Type = AuthMessageType.Success});
			////}

			////user.RoleID = UnitOfWork.RoleRepository.GetDefaultRole().RoleID;
			//UnitOfWork.UserRepository.Update(user);
			//UnitOfWork.Save();

			//return View("Message", new MessageModel() { Title = "Félicitation, " + user.CompleteName, Content = "Votre compte est maintenant activé! Vous pouvez maintenant vous connecter et réserver une place.", Type = AuthMessageType.Success });

			throw new NotImplementedException();
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