using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
		public async Task<ActionResult> Login(LoginModel loginInfo)
		{
			if (ModelState.IsValid)
			{
				TryLoginResult realUser = UserService.TryLogin(loginInfo.Email, loginInfo.Password, false);

				if (realUser.HasSucceeded) {
					return RedirectToReturnUrl(loginInfo.ReturnURL);
				}
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