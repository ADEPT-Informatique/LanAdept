using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LanAdept.Emails;
using LanAdept.Views.Auth.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdept.Controllers
{
	public class AuthController : Controller
	{
		[AuthorizeGuestOnly]
		public ActionResult Index()
		{
			return RedirectToAction("Login");
		}

		[AuthorizeGuestOnly]
		public ActionResult Login(string returnURL)
		{
			return View();
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel loginInfo)
		{
			if (ModelState.IsValid)
			{
				TryLoginResult realUser = UserService.TryLogin(loginInfo.Email, loginInfo.Password, loginInfo.RememberMe);

				if (realUser.HasSucceeded)
					return RedirectToReturnUrl(loginInfo.ReturnURL);
				else
					ModelState.AddModelError("", realUser.Reason);
			}

			loginInfo.Password = "";

			//Temps d'attente aléatoire après un erreur pour aider à prévenir le bruteforce
			Random rand = new Random();
			await Task.Delay(rand.Next(1500, 6000)); //Entre 1.5 et 6 secondes d'attente

			return View(loginInfo);
		}

		[Authorize]
		public ActionResult Logout()
		{
			UserService.Logout();
			return RedirectToAction("Index", "Home");
		}

		[AuthorizeGuestOnly]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				User newUser = UserService.CreateUser(model.Email, model.Password, model.CompleteName);

				UnitOfWork.Current.UserRepository.Insert(newUser);
				UnitOfWork.Current.Save();

				ConfirmationEmail email = new ConfirmationEmail();
				email.User = newUser;
				email.Send();

				MessageModel result = new MessageModel();
				result.Title = "Vous êtes maintenant inscrit!";
				result.Content = "Vous devez maintenant <strong>confirmer votre email</strong>. Une fois que ce sera fait, vous pourrez réserver une place pour participer au LAN de l'Adept.";
				result.Type = AuthMessageType.Success;

				return View("Message", result);
			}

			model.Password = null;
			model.PasswordConfirmation = null;

			return View(model);
		}

		[AuthorizeGuestOnly]
		public ActionResult Confirm(string id)
		{
			User user = UnitOfWork.Current.UserRepository.GetUserByBarCode(id);

			if (user == null)
			{
				return View("Message", new MessageModel() { Title = "Une erreur est survenue", Content = "Ce lien n'est pas valide. Si vous continuez de voir cette erreur, contactez un administrateur.", Type = AuthMessageType.Error });
			}

			if (user.RoleID != UnitOfWork.Current.RoleRepository.GetUnconfirmedRole().RoleID)
			{
				return View("Message", new MessageModel() { Title = "Votre compte est déjà actif", Content = "Vous pouvez maintenant vous connecter et réserver une place.", Type = AuthMessageType.Success});
			}

			user.RoleID = UnitOfWork.Current.RoleRepository.GetDefaultRole().RoleID;
			UnitOfWork.Current.UserRepository.Update(user);
			UnitOfWork.Current.Save();

			return View("Message", new MessageModel() { Title = "Félicitation, " + user.CompleteName, Content = "Votre compte est maintenant activé! Vous pouvez maintenant vous connecter et réserver une place.", Type = AuthMessageType.Success });
		}

		private ActionResult RedirectToReturnUrl(string returnURL)
		{
			if (!string.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
			{
				return Redirect(returnURL);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
	}
}