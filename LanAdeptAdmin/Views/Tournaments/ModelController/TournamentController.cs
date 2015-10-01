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
		public ActionResult Create([Bind(Include = "GameID, StartTime")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = new Tournament();

				tournament.StartTime = tournamentModel.StartTime;
				tournament.GameID = tournamentModel.GameID;

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
				return RedirectToAction("Index");
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
				team.Users.Clear();
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
			int tournamentID = uow.TeamRepository.GetByID(TeamId).TournamentID;
			uow.TeamRepository.Delete(uow.TeamRepository.GetByID(TeamId));
			uow.Save();
			return RedirectToAction("Details", new { id = tournamentID });
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
