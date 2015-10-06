using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using LanAdeptAdmin.Views.Tournaments.ModelController;

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			List<TournamentModel> tournamentModelList = new List<TournamentModel>();
			IEnumerable<Tournament> tournaments = uow.TournamentRepository.Get();
			foreach (Tournament tournament in tournaments)
			{
				tournamentModelList.Add(new TournamentModel(tournament));
			}
			return View(tournamentModelList);
		}

		[Authorize]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TournamentModel tournament = new TournamentModel(uow.TournamentRepository.GetByID(id));
			if (tournament == null)
			{
				return HttpNotFound();
			}
			return View(tournament);
		}

		[Authorize]
		public ActionResult Create()
		{
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name");
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "GameID, StartTime,MaxPlayerPerTeam")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = new Tournament();

				tournament.GameID = tournamentModel.GameID;
				tournament.StartTime = tournamentModel.StartTime;
				tournament.MaxPlayerPerTeam = tournamentModel.MaxPlayerPerTeam;

				tournament.CreationDate = DateTime.Now;

				uow.TournamentRepository.Insert(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournamentModel.GameID);
			return View(tournamentModel);
		}

		[Authorize]
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TournamentModel tournament = new TournamentModel(uow.TournamentRepository.GetByID(id));
			if (tournament == null)
			{
				return HttpNotFound();
			}
			ViewBag.Id = tournament.Id;
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournament.Game.GameID);
			return View(tournament);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "GameID, MaxPlayerPerTeam, StartTime, Id, IsStarted, IsOver, Info")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = uow.TournamentRepository.GetByID(tournamentModel.Id);

				tournament.Game = tournamentModel.Game;
				tournament.MaxPlayerPerTeam = tournamentModel.MaxPlayerPerTeam;
				tournament.StartTime = tournamentModel.StartTime;
				tournament.IsStarted = tournamentModel.IsStarted;
				tournament.IsOver = tournamentModel.IsOver;
				tournament.Info = tournamentModel.Info;

				uow.TournamentRepository.Update(tournament);
				uow.Save();
				return RedirectToAction("Details", new { id = tournament.TournamentID});
			}
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournamentModel.Game.GameID);
			return View(tournamentModel);
		}

		[Authorize]
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TournamentModel tournament = new TournamentModel(uow.TournamentRepository.GetByID(id));
			if (tournament == null)
			{
				return HttpNotFound();
			}
			return View(tournament);
		}

		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Tournament tournament = uow.TournamentRepository.GetByID(id);

			while (tournament.Teams.Count != 0)
			{
				Team team = tournament.Teams.First();
				team.GamerTags.Clear();
                uow.TeamRepository.Delete(team);
			}

			uow.TournamentRepository.Delete(tournament);
			uow.Save();
			return RedirectToAction("Index");
		}

		[Authorize]
		public ActionResult DeleteTeam(int? TeamId)
		{
			if (TeamId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TeamModel team = new TeamModel(uow.TeamRepository.GetByID(TeamId));
			if (team == null)
			{
				return HttpNotFound();
			}
			return View(team);
		}

		[Authorize]
		[HttpPost, ActionName("DeleteTeam")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteTeamConfirmed(int TeamId)
		{
			Team team = uow.TeamRepository.GetByID(TeamId);

			int tournamentID = team.TournamentID;

			team.GamerTags.Clear();
			uow.TeamRepository.Delete(uow.TeamRepository.GetByID(TeamId));

			uow.Save();
			return RedirectToAction("Details", new { id = tournamentID });
		}

		[Authorize]
		public ActionResult DetailsTeam(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TeamModel team = new TeamModel(uow.TeamRepository.GetByID(id));
			if (team == null)
			{
				return HttpNotFound();
			}
			return View(team);
		}

		[Authorize]
		public ActionResult KickPlayer(int? gamerTagId, int? teamId)
		{
			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(teamId);

			if (team.TeamLeaderTag == gamerTag || team.GamerTags.Count == 1)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas kicker le team leader.";
                return RedirectToAction("DetailsTeam", new { id = teamId });
			}
			else
			{
				team.GamerTags.Remove(gamerTag);

				uow.TeamRepository.Update(team);
				uow.GamerTagRepository.Update(gamerTag);
				uow.Save();
			}

			return RedirectToAction("DetailsTeam", new { id = teamId });
		}

		[Authorize]
		public ActionResult EditTeam(int? teamId)
		{
			if (teamId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			TeamModel team = new TeamModel(uow.TeamRepository.GetByID(teamId));
			if (team == null)
			{
				return HttpNotFound();
			}
			return View(team);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditTeam([Bind(Include = "TeamId,Name,Tag,StartTime,IsComplete,IsReady,IsConfirmed")] TeamModel teamModel)
		{
			if (ModelState.IsValid)
			{
				Team team = (uow.TeamRepository.GetByID(teamModel.TeamID));

				team.Name = teamModel.Name;
				team.Tag = teamModel.Tag;
				team.IsComplete = teamModel.IsComplete;
				team.IsReady = teamModel.IsReady;
				team.IsConfirmed = teamModel.IsConfirmed;

				uow.TeamRepository.Update(team);
				uow.Save();

				return RedirectToAction("DetailsTeam", new { id = teamModel.TeamID });
			}
			return View(teamModel);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				uow.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
