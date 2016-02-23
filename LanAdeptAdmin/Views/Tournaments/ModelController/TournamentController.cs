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
using Microsoft.AspNet.Identity.Owin;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.Model.Tournaments;

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Create([Bind(Include = "Game, StartTime, MaxPlayerPerTeam")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = new Tournament();

				tournament.Game = tournamentModel.Game;
				tournament.StartTime = tournamentModel.StartTime;
				tournament.MaxPlayerPerTeam = tournamentModel.MaxPlayerPerTeam;

				tournament.CreationDate = DateTime.Now;

				uow.TournamentRepository.Insert(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}
			return View(tournamentModel);
		}

		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public ActionResult Edit([Bind(Include = "Game, MaxPlayerPerTeam, StartTime, Id, IsStarted, IsOver, Info")] TournamentModel tournamentModel)
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

			return View(tournamentModel);
		}

		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[HttpPost, ActionName("DeleteTeam")]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult EditTeam([Bind(Include = "TeamId,Name,Tag,StartTime,IsConfirmed")] TeamModel teamModel)
		{
			if (ModelState.IsValid)
			{
				Team team = (uow.TeamRepository.GetByID(teamModel.TeamID));

				team.Name = teamModel.Name;
				team.Tag = teamModel.Tag;
				team.IsConfirmed = teamModel.IsConfirmed;

				uow.TeamRepository.Update(team);
				uow.Save();

				return RedirectToAction("DetailsTeam", new { id = teamModel.TeamID });
			}
			return View(teamModel);
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult Publish(int? id)
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

			tournament.IsPublished = true;
			tournament.IsStarted = false;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult CancelPublish(int? id)
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

			tournament.IsPublished = false;
			tournament.IsStarted = false;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult Start(int? id)
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

			tournament.IsStarted = true;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
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

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult TeamReady(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = uow.TeamRepository.GetByID(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            team.IsConfirmed = true;
			uow.TeamRepository.Update(team);
			uow.Save();

			return RedirectToAction("DetailsTeam", new { id = team.TeamID });
        }

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public ActionResult TeamNotReady(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = uow.TeamRepository.GetByID(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            team.IsConfirmed = false;
			uow.TeamRepository.Update(team);
			uow.Save();

            return RedirectToAction("DetailsTeam", new { id = team.TeamID });
        }
    }
}
