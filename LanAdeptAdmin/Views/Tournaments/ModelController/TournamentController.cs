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
using LanAdeptCore.Attribute.Authorization;

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[AuthorizePermission("admin.tournament.index")]
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

		[AuthorizePermission("admin.tournament.details")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tournament tournament = uow.TournamentRepository.GetByID(id);
			if (tournament == null)
			{
				return HttpNotFound();
			}

			return View(new TournamentModel(tournament));
		}

		[AuthorizePermission("admin.tournament.create")]
		public ActionResult Create()
		{
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name");
			return View();
		}

		[AuthorizePermission("admin.tournament.create")]
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

		[AuthorizePermission("admin.tournament.edit")]
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

			return View(tournament);
		}

		[AuthorizePermission("admin.tournament.edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaxPlayerPerTeam, StartTime, Id, IsStarted, IsOver, Info")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = uow.TournamentRepository.GetByID(tournamentModel.Id);

				tournament.MaxPlayerPerTeam = tournamentModel.MaxPlayerPerTeam;
				tournament.StartTime = tournamentModel.StartTime;
				tournament.IsStarted = tournamentModel.IsStarted;
				tournament.IsOver = tournamentModel.IsOver;
				tournament.Info = tournamentModel.Info;

				uow.TournamentRepository.Update(tournament);
				uow.Save();
				return RedirectToAction("Details", new { id = tournament.TournamentID});
			}

			return View(tournamentModel);
		}

		[AuthorizePermission("admin.tournament.delete")]
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

		[AuthorizePermission("admin.tournament.delete")]
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

		[AuthorizePermission("admin.tournament.team.delete")]
		public ActionResult DeleteTeam(int? id)
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

		[AuthorizePermission("admin.tournament.team.delete")]
		[HttpPost, ActionName("DeleteTeam")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteTeamConfirmed(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

			int tournamentID = team.TournamentID;

			team.GamerTags.Clear();

			IEnumerable<Demande> demandes = uow.DemandeRepository.Get();

			foreach (Demande demande in demandes)
			{
				if (demande.Team == team)
				{
					uow.DemandeRepository.Delete(demande.DemandeID);
				}
			}

			uow.TeamRepository.Delete(uow.TeamRepository.GetByID(id));

			uow.Save();
			return RedirectToAction("Details", new { id = tournamentID });
		}

		[AuthorizePermission("admin.tournament.team.details")]
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

		[AuthorizePermission("admin.tournament.team.kick")]
		public ActionResult KickPlayer(int? gamerTagId, int? teamId)
		{
			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(teamId);

			if (team.TeamLeaderTag == gamerTag || team.GamerTags.Count == 1)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas exclure le team leader.";
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

		[AuthorizePermission("admin.tournament.team.edit")]
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

		[AuthorizePermission("admin.tournament.team.edit")]
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

		[AuthorizePermission("admin.tournament.edit")]
		public ActionResult Start(int? id)
		{
			if(id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tournament tournament = uow.TournamentRepository.GetByID(id);
			if (tournament == null)
			{
				return HttpNotFound();
			}

			tournament.IsStarted = true;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[AuthorizePermission("admin.tournament.edit")]
		public ActionResult Stop(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tournament tournament = uow.TournamentRepository.GetByID(id);
			if (tournament == null)
			{
				return HttpNotFound();
			}

			tournament.IsStarted = false;
			tournament.IsOver = true;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[AuthorizePermission("admin.tournament.edit")]
		public ActionResult CancelStart(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tournament tournament = uow.TournamentRepository.GetByID(id);
			if (tournament == null)
			{
				return HttpNotFound();
			}

			tournament.IsStarted = false;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}
	}
}
