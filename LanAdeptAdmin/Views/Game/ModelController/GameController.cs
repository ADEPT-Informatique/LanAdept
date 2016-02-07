using LanAdeptAdmin.Views.Game.ModelController;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Controllers
{
	public class GameController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Create([Bind(Include = "GameID, Name, Description")] GameModel gameModel)
		{
			if (ModelState.IsValid)
			{
				LanAdeptData.Model.Game game = new LanAdeptData.Model.Game();

				game.GameID = gameModel.GameID;
				game.Name = gameModel.Name;

				uow.GameRepository.Insert(game);
				uow.Save();
				return RedirectToAction("Index");
			}

			return View(gameModel);
		}

		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Edit([Bind(Include = "GameID, Name, Description")] GameModel gameModel)
		{
			if (ModelState.IsValid)
			{
				LanAdeptData.Model.Game game = new LanAdeptData.Model.Game();

				game.GameID = gameModel.GameID;
				game.Name = gameModel.Name;

				uow.GameRepository.Update(game);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View(gameModel);
		}

		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult DeleteConfirmed(int id)
		{
			Game game = uow.GameRepository.GetByID(id);

			while (game.Tournaments.Count != 0)
			{
				Tournament tournament = game.Tournaments.First();

				while (tournament.Teams.Count != 0)
				{
					Team team = tournament.Teams.First();
					team.GamerTags.Clear();
					uow.TeamRepository.Delete(team);
				}

				uow.TournamentRepository.Delete(tournament);
			}

			uow.GameRepository.Delete(game);

			uow.Save();
			return RedirectToAction("Index");
		}
	}
}