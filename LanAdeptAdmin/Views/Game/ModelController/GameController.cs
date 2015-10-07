using LanAdeptAdmin.Views.Game.ModelController;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Views.Games.ModelController
{
	public class GameController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[AuthorizePermission("admin.game.index")]
		public ActionResult Index()
		{
			List<GameModel> gameModels = new List<GameModel>();
			IEnumerable<LanAdeptData.Model.Game> games = uow.GameRepository.Get();
			foreach (LanAdeptData.Model.Game game in games)
			{
				gameModels.Add(new GameModel(game));
			}
			return View(gameModels);
		}

		[AuthorizePermission("admin.game.details")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			GameModel game = new GameModel(uow.GameRepository.GetByID(id));
			if (game == null)
			{
				return HttpNotFound();
			}
			return View(game);
		}

		[AuthorizePermission("admin.game.create")]
		public ActionResult Create()
		{
			return View();
		}

		[AuthorizePermission("admin.game.create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "GameID, Name, Description")] GameModel gameModel)
		{
			if (ModelState.IsValid)
			{
				LanAdeptData.Model.Game game = new LanAdeptData.Model.Game();

				game.GameID = gameModel.GameID;
				game.Name = gameModel.Name;
				game.Description = gameModel.Description;

				uow.GameRepository.Insert(game);
				uow.Save();
				return RedirectToAction("Index");
			}

			return View(gameModel);
		}

		[AuthorizePermission("admin.game.edit")]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			GameModel game = new GameModel(uow.GameRepository.GetByID(id));
			if (game == null)
			{
				return HttpNotFound();
			}
			//ViewBag.GameID = game.GameID;

			return View(game);
		}

		[AuthorizePermission("admin.game.edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "GameID, Name, Description")] GameModel gameModel)
		{
			if (ModelState.IsValid)
			{
				LanAdeptData.Model.Game game = new LanAdeptData.Model.Game();

				game.GameID = gameModel.GameID;
				game.Name = gameModel.Name;
				game.Description = gameModel.Description;

				uow.GameRepository.Update(game);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View(gameModel);
		}

		[AuthorizePermission("admin.game.delete")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			GameModel game = new GameModel(uow.GameRepository.GetByID(id));
			if (game == null)
			{
				return HttpNotFound();
			}
			return View(game);
		}

		[AuthorizePermission("admin.game.delete")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			uow.GameRepository.Delete(uow.GameRepository.GetByID(id));
			uow.Save();
			return RedirectToAction("Index");
		}
	}
}