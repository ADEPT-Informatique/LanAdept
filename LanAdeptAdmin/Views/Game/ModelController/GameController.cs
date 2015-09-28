using LanAdeptAdmin.Views.Game.ModelController;
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
			List<GameModel> gameModels = new List<GameModel>();
			IEnumerable<LanAdeptData.Model.Game> games = uow.GameRepository.Get();
			foreach (LanAdeptData.Model.Game game in games)
			{
				gameModels.Add(new GameModel(game));
			}
			return View(gameModels);
		}

		[Authorize]
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

		[Authorize]
		public ActionResult Create()
		{
			return View();
		}

		[Authorize]
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

		[Authorize]
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

		[Authorize]
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

		[Authorize]
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

		[Authorize]
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