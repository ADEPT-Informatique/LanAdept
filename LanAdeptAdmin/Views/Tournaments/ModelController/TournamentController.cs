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
using LanAdeptCore.Service;
using System.Threading.Tasks;
using LanAdeptCore.Service.Challonge;
using LanAdeptCore.Service.Challonge.Request;
using LanAdeptData.Model.Tournaments;
using LanAdeptData.Model.Users;
using LanAdeptCore.Service;
using LanAdeptCore.Service.Challonge.Response;

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

        private IEnumerable<SelectListItem> TournamentTypes
        {
            get 
            {
                List<SelectListItem> types = new List<SelectListItem>();
                types.Add(new SelectListItem() { Value = ((int)TounamentType.Single).ToString(), Text = TounamentType.Single.ToString() });
                types.Add(new SelectListItem() { Value = ((int)TounamentType.Double).ToString(), Text = TounamentType.Double.ToString(), Selected = true });
                types.Add(new SelectListItem() { Value = ((int)TounamentType.RoundRobin).ToString(), Text = TounamentType.RoundRobin.ToString() });
                types.Add(new SelectListItem() { Value = ((int)TounamentType.Swiss).ToString(), Text = TounamentType.Swiss.ToString() });
                return types;
            }
        }

        #region Tournament

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
            ViewBag.Types = TournamentTypes;
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LanAuthorize(Roles = "tournamentAdmin")]
        public async Task<ActionResult> Create([Bind(Include = "Game, StartTime, MaxPlayerPerTeam, IsChallonge, Type")] TournamentCreateModel tournamentModel)
        {
            if (ModelState.IsValid)
            {
                Tournament tournament = new Tournament();

                tournament.Game = tournamentModel.Game;
                tournament.StartTime = tournamentModel.StartTime;
                tournament.MaxPlayerPerTeam = tournamentModel.MaxPlayerPerTeam;

                tournament.CreationDate = DateTime.Now;

                if (tournamentModel.IsChallonge)
                {
                    TournamentRequest request = new TournamentRequest();
                    request.Name = tournamentModel.Game;
                    request.Type = tournamentModel.Type;
                    request.Description = "Tournoi du lan de l'adept " + DateTime.Now.Year + " pour le jeu " + tournamentModel.Game;
                    TournamentResponse response = await ChallongeService.CreateTournament(request);

                    if (response.HasError)
                    {
                        return View(tournamentModel);
                    }

                    tournament.ChallongeUrl = response.Tournament.Url;   
                }

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
            ViewBag.Types = TournamentTypes;
			return View(tournament);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public async Task<ActionResult> Edit([Bind(Include = "Game, MaxPlayerPerTeam, StartTime, Id, IsStarted, IsOver, Info, IsChallonge, Type")] TournamentModel tournamentModel)
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

                if (tournamentModel.IsChallonge)
                {
                    if (tournament.ChallongeUrl == null)
                    {
                        TournamentRequest request = new TournamentRequest();
                        request.Name = tournamentModel.Game;
                        request.Type = TounamentType.Double;
                        request.Description = "Tournoi du lan de l'adept " + DateTime.Now.Year + " pour le jeu " + tournamentModel.Game;
                        TournamentResponse response = await ChallongeService.CreateTournament(request);

                        if (response.HasError)
                        {
                            return RedirectToAction("Details", new { id = tournament.TournamentID });
                        }

                        tournament.ChallongeUrl = response.Tournament.Url;

                        foreach (var team in tournament.Teams)
                        {
                            ParticipantRequest partRequest = new ParticipantRequest();
                            partRequest.TournamentUrl = tournament.ChallongeUrl;
                            partRequest.Name = team.Name;
                            partRequest.Misc = team.TeamLeaderTag.Gamertag;

                            ParticipantResponse partResponse = await ChallongeService.CreateParticipant(partRequest);

                            if (partResponse.HasError)
                            {
                                return RedirectToAction("Details", new { id = tournament.TournamentID });
                            }

                            team.ChallongeID = partResponse.Participant.ParticipantId;
                        }
                    }
                    else if(tournament.Game != tournamentModel.Game)
                    {
                        SimpleResponse response = await ChallongeService.UpdateTournament(tournament.ChallongeUrl, tournamentModel.Game);

                        if (response.HasError)
                        {
                            return RedirectToAction("Details", new { id = tournament.TournamentID });
                        }
                    }
                }
                else if(tournament.ChallongeUrl != null)
                {
                    SimpleResponse response = await ChallongeService.DeleteTournament(tournament.ChallongeUrl);
                    if (response.HasError)
                    {
                        return RedirectToAction("Details", new { id = tournament.TournamentID });
                    }
                    tournament.ChallongeUrl = null;
                }

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

			Tournament tournament = uow.TournamentRepository.GetByID(id);
			if (tournament == null)
			{
				return HttpNotFound();
			}

            return View(new TournamentModel(tournament));
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[LanAuthorize(Roles = "tournamentAdmin")]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			Tournament tournament = uow.TournamentRepository.GetByID(id);

            if (tournament == null)
            {
                return HttpNotFound();
            }

			while (tournament.Teams.Count != 0)
			{
				Team team = tournament.Teams.First();
				team.GamerTags.Clear();
                uow.TeamRepository.Delete(team);
			}

            if (tournament.ChallongeUrl != null)
            {
                SimpleResponse response = await ChallongeService.DeleteTournament(tournament.ChallongeUrl);
                if (response.HasError)
                {
                    return RedirectToAction("Index");
                }
            }

			uow.TournamentRepository.Delete(tournament);
			uow.Save();
			return RedirectToAction("Index");
		}

        #endregion

        #region Team

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
		public async Task<ActionResult> DeleteTeamConfirmed(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

            if (team.ChallongeID != null)
            {
                SimpleResponse response = await ChallongeService.DeleteParticipant(team.Tournament.ChallongeUrl, team.ChallongeID.Value);

                if (response.HasError)
                {
                    return RedirectToAction("Details", new { id = team.TournamentID });
                }
            }

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
			return RedirectToAction("Details", new { id = team.TournamentID });
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
		public ActionResult PromotePlayer(int? gamerTagId, int? teamId)
		{
			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(teamId);

			if (team.TeamLeaderTag == gamerTag || team.GamerTags.Count == 1)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas promouvoir le team leader.";
				return RedirectToAction("DetailsTeam", new { id = teamId });
			}
			else
			{

				team.TeamLeaderTag = gamerTag;

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
		public async  Task<ActionResult> EditTeam([Bind(Include = "TeamId,Name,Tag,StartTime,IsConfirmed")] TeamModel teamModel)
		{
			if (ModelState.IsValid)
			{
				Team team = (uow.TeamRepository.GetByID(teamModel.TeamID));

                if (team.ChallongeID != null && team.Name != teamModel.Name)
                {
                    SimpleResponse response = await ChallongeService.UpdateParticipant(team.ChallongeID.Value, team.Tournament.ChallongeUrl, teamModel.Name);

                    if (response.HasError)
                    {
                        return RedirectToAction("DetailsTeam", new { id = teamModel.TeamID });
                    }
                }

				team.Name = teamModel.Name;
				team.Tag = teamModel.Tag;
				team.IsConfirmed = teamModel.IsConfirmed;

				uow.TeamRepository.Update(team);
				uow.Save();

				return RedirectToAction("DetailsTeam", new { id = teamModel.TeamID });
			}
			return View(teamModel);
		}

        #endregion

        #region Setting

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
		public async Task<ActionResult> Start(int? id)
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

            if (tournament.ChallongeUrl != null)
            {
                if (tournament.Teams.Count < 2)
                {
                    return RedirectToAction("Details", new { id = id });
                }

                SimpleResponse responseRand = await ChallongeService.RandomizeTournament(tournament.ChallongeUrl);

                if (responseRand.HasError)
                {
                    return RedirectToAction("Details", new { id = id });
                }

                SimpleResponse responseStart = await ChallongeService.StartTournament(tournament.ChallongeUrl);

                if (responseStart.HasError)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

			tournament.IsStarted = true;
			tournament.IsOver = false;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public async Task<ActionResult> Stop(int? id)
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

            if (tournament.ChallongeUrl != null)
            {
                SimpleResponse response = await ChallongeService.FinalizeTournament(tournament.ChallongeUrl);

                if (response.HasError)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

			tournament.IsStarted = false;
			tournament.IsOver = true;

			uow.TournamentRepository.Update(tournament);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize(Roles = "tournamentAdmin, tournamentMod")]
		public async Task<ActionResult> CancelStart(int? id)
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

            if (tournament.ChallongeUrl != null)
            {
                SimpleResponse response = await ChallongeService.ResetTournament(tournament.ChallongeUrl);

                if (response.HasError)
                {
                    return RedirectToAction("Details", new { id = id });
                }
            }

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

        #endregion
    }
}
