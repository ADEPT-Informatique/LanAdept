using LanAdeptData.DAL;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LanAdeptAdmin.Views.Games.ModelController
{
	public class GameController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			//ViewBag.Games = uow.GameRepository.Get();
			return View(uow.GameRepository.Get());
		}

		[Authorize]
		public ActionResult Details(int? id) {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Game game = uow.GameRepository.GetByID(id);
			if (game == null)
			{
				return HttpNotFound();
			}

			return View(game);
		}

		[Authorize]
		public ActionResult Create()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "GameID, Name, Description")] Game game)
		{
			if (ModelState.IsValid)
			{
				uow.GameRepository.Insert(game);
				uow.Save();
				return RedirectToAction("Index");
			}

			return View(game);
		}

		[Authorize]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Game game = uow.GameRepository.GetByID(id);
			if (game == null)
			{
				return HttpNotFound();
			}

			return View(game);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "GameID, Name, Description")] Game game)
		{
			if (ModelState.IsValid)
			{
				uow.GameRepository.Update(game);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View(game);
		}

		[Authorize]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Game game = uow.GameRepository.GetByID(id);
			if (game == null)
			{
				return HttpNotFound();
			}
			return View(game);
		}

		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Game game = uow.GameRepository.GetByID(id);
			uow.GameRepository.Delete(game);
			uow.Save();
			return RedirectToAction("Index");
		}
	}
}