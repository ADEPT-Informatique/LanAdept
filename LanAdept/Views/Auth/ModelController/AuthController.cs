using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Auth.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Service;
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

		[AuthorizeGuestOnly]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AuthorizeGuestOnly]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				User newUser = UserService.CreateUser(model.Email, model.Password, model.CompleteName);

				UnitOfWork.Current.UserRepository.Insert(newUser);
				UnitOfWork.Current.Save();

				MessageModel result = new MessageModel();
				result.Titre = "Vous êtes maintenant inscrit!";
				result.Contenu = "Vous devez maintenant <strong>confirmer votre email</strong>. Une fois que ce sera fait, vous pourrez réserver une place pour participer au LAN de l'Adept.";
				result.Type = AuthMessageType.Success;

				return View("Message", result);
			}

			model.Password = null;
			model.PasswordConfirmation = null;

			return View(model);
		}
    }
}