using LanAdeptData.DAL;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using LanAdeptData.Model;
using LanAdeptCore.Service;
using System.Data;
using LanAdeptCore.Attribute.Authorization;
using System.Linq;
using LanAdept.Views.Tournaments.ModelController;
using System;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using LanAdeptData.Model.Users;
using LanAdeptData.Model.Tournaments;

namespace LanAdept.Controllers
{
	public class TournamentController : Controller
	{
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[AllowAnonymous]
		public ActionResult Index()
		{
			List<TournamentModel> tournamentModels = new List<TournamentModel>();
			IEnumerable<Tournament> tournaments = uow.TournamentRepository.Get();

			if (tournaments.Count() == 0)
			{
				return View("TournamentComing");
			}

			foreach (Tournament tournament in tournaments)
			{
				TournamentModel tournamentModel = new TournamentModel(tournament);
				List<TeamModel> teamModels = new List<TeamModel>();
				foreach (Team team in tournament.Teams)
				{
					TeamModel teamModel = new TeamModel(team);
					teamModels.Add(teamModel);
				}
				tournamentModel.Teams = teamModels;
				tournamentModels.Add(tournamentModel);
			}

			return View(tournamentModels.OrderBy(t => t.StartTime).ThenBy(t => t.Game.Name));
		}

		[AllowAnonymous]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Index");
			}

			Tournament tournament = uow.TournamentRepository.GetByID(id);


			if (tournament == null)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Index");
			}


			TournamentModel tournamentModel = new TournamentModel(tournament);

			tournamentModel.CanAddTeam = UserService.IsUserLoggedIn();
			tournamentModel.IsTeamLeader = UserService.IsTeamLeader(tournament.TournamentID);

			List<TeamModel> teamModels = new List<TeamModel>();
			foreach (Team team in tournament.Teams)
			{
				TeamModel teamModel = new TeamModel(team);

				if (UserService.IsUserLoggedIn())
				{
					User loggedInUser = UserService.GetLoggedInUser();
					if (team.TeamLeaderTag.UserID == loggedInUser.Id)
					{
						tournamentModel.IsTeamLeader = true;
						tournamentModel.CanAddTeam = false;
						teamModel.IsMyTeamForTeamLeader = true;
						teamModel.IsTeamDemande = false;
						teamModel.IsMyTeam = false;
					}
					else
					{
						foreach (GamerTag gamer in team.GamerTags)
						{
							if (gamer.User.Id == loggedInUser.Id)
							{
								tournamentModel.CanAddTeam = false;
								teamModel.IsMyTeamForTeamLeader = false;
								teamModel.IsTeamDemande = false;
								teamModel.IsMyTeam = true;
								break;
							}
						}

						if (!teamModel.IsMyTeam)
						{
							foreach (Demande demande in team.Demandes)
							{
								if (demande.GamerTag.UserID == loggedInUser.Id)
								{
									tournamentModel.CanAddTeam = false;
									teamModel.IsMyTeamForTeamLeader = false;
									teamModel.IsTeamDemande = true;
									teamModel.IsMyTeam = false;
									break;
								}
							}
						}
					}
				}

				teamModel.Gamertags = teamModel.Gamertags.OrderByDescending(g => g.GamerTagID == team.TeamLeaderTag.GamerTagID).ThenBy(g => g.Gamertag);

				teamModels.Add(teamModel);
			}

			tournamentModel.Teams = teamModels.OrderBy(t => t.Name);

			User user = UserService.GetLoggedInUser();
			if (user != null)
			{
				tournamentModel.IsConnected = true;
				tournamentModel.GamerTags = uow.GamerTagRepository.GetByUser(user);
				tournamentModel.UserTeam = uow.TeamRepository.UserTeamInTournament(user, tournament);
			}

			return View(tournamentModel);
		}

		public ActionResult Addteam(int id)
		{
			Tournament tournament = uow.TournamentRepository.GetByID(id);

			if (tournament.IsStarted || tournament.IsOver)
			{
				TempData["Error"] = "Il n'est pas possible d'ajouter une équipe pour le moment";
				return RedirectToAction("Details", new { id = tournament.TournamentID });
			}

			User loggedInUser = UserService.GetLoggedInUser();

			foreach (Team team in tournament.Teams)
			{
				if (team.TeamLeaderTag.User.Id == loggedInUser.Id)
				{
					TempData["Error"] = "Vous ne pouvez pas ajouter une équipe car vous en avez déjà une.";
					return RedirectToAction("Details", new { id = team.Tournament.TournamentID });
				}
			}

			foreach (Team team in tournament.Teams)
			{
				foreach (Demande demande in team.Demandes)
				{
					if (demande.GamerTag.User.Id == loggedInUser.Id)
					{
						TempData["Error"] = "Vous ne pouvez pas ajouter une équipe car vous avez envoyé une demande pour rejoindre une équipe.";
						return RedirectToAction("Details", new { id = tournament.TournamentID });
					}
				}
			}

			AddTeamModel model = new AddTeamModel();
			model.Tournament = tournament;
			model.TournamentID = model.Tournament.TournamentID;
			model.GamerTags = uow.GamerTagRepository.GetByUser(UserService.GetLoggedInUser());
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Addteam(AddTeamModel teamModel)
		{
			Tournament tournament = uow.TournamentRepository.GetByID(teamModel.TournamentID);

			if (tournament.IsStarted || tournament.IsOver)
			{
				TempData["Error"] = "Il n'est pas possible d'ajouter une équipe pour le moment";
				return RedirectToAction("Details", new { id = tournament.TournamentID });
			}

			User loggedInUser = UserService.GetLoggedInUser();

			foreach (Team team in tournament.Teams)
			{
				if (team.TeamLeaderTag.User.Id == loggedInUser.Id)
				{
					TempData["Error"] = "Vous ne pouvez pas ajouter une équipe car vous en avez déjà une.";
					return RedirectToAction("Details", new { id = team.Tournament.TournamentID });
				}
			}

			foreach (Team team in tournament.Teams)
			{
				foreach (Demande demande in team.Demandes)
				{
					if (demande.GamerTag.User.Id == loggedInUser.Id)
					{
						TempData["Error"] = "Vous ne pouvez pas ajouter une équipe car vous avez envoyé une demande pour rejoindre une équipe.";
						return RedirectToAction("Details", new { id = tournament.TournamentID });
					}
				}
			}

			if (ModelState.IsValid)
			{
				Team team = new Team();
				team.Name = teamModel.Name;
				team.Tag = teamModel.Tag;
				team.TeamLeaderTag = uow.GamerTagRepository.GetByUserAndGamerTagID(loggedInUser, teamModel.GamerTagId);

				List<GamerTag> listGamerTags = new List<GamerTag>();
				listGamerTags.Add(team.TeamLeaderTag);
				team.GamerTags = listGamerTags;
				team.Tournament = uow.TournamentRepository.GetByID(teamModel.TournamentID);

				uow.TeamRepository.Insert(team);

				if (team.Tournament.Teams == null)
				{
					ICollection<Team> teamList;
					teamList = new List<Team>();
					teamList.Add(team);
					team.Tournament.Teams = teamList;
				}
				else
				{
					team.Tournament.Teams.Add(team);
				}

				uow.TournamentRepository.Update(team.Tournament);

				uow.Save();
				return RedirectToAction("Details", new { id = team.Tournament.TournamentID });
			}

			teamModel.GamerTags = uow.GamerTagRepository.GetByUser(UserService.GetLoggedInUser());
			return View(teamModel);
		}

		public ActionResult AddGamerTag(string gamertag)
		{
			User user = UserService.GetLoggedInUser();

			if (!uow.GamerTagRepository.HasSameGamerTag(user, gamertag))
			{
				GamerTag gamerTag = new GamerTag();
				gamerTag.Gamertag = gamertag;
				gamerTag.User = user;

				uow.GamerTagRepository.Insert(gamerTag);
				uow.Save();

				return Json(new GamerTagResponse() { HasError = false, ErrorMessage = "", GamerTagID = gamerTag.GamerTagID, Gamertag = gamerTag.Gamertag }, JsonRequestBehavior.AllowGet);
			}

			return Json(new GamerTagResponse() { HasError = true, ErrorMessage = "Vous avez déja un GamerTag avec ce nom", GamerTagID = 0, Gamertag = gamertag }, JsonRequestBehavior.AllowGet); ;
		}

		public ActionResult JoinTeam(JoinTeamModel model)
		{
			if (model.GamerTagID == null || model.TournamentID == null || model.TeamID == null)
			{
				//TODO : Add client error
				return HttpNotFound();
			}

			Tournament tournament = uow.TournamentRepository.GetByID(model.TournamentID);
			if (tournament.IsStarted || tournament.IsOver)
			{
				return RedirectToAction("Details", new { id = tournament.TournamentID });
			}


			User user = UserService.GetLoggedInUser();
			if (uow.TeamRepository.UserTeamInTournament(user, tournament) != null)
			{
				return RedirectToAction("Details", new { id = tournament.TournamentID });
			}


			foreach (Demande item in uow.DemandeRepository.GetByTeamId(model.TeamID))
			{
				if (item.GamerTag.GamerTagID == model.GamerTagID)
				{
					return RedirectToAction("Details", new { id = tournament.TournamentID });
				}
			}

			Demande demande = new Demande();

			GamerTag gamerTag = uow.GamerTagRepository.GetByUserAndGamerTagID(user, model.GamerTagID.Value);
			if (gamerTag == null)
			{
				//TODO : Add client error
				return HttpNotFound();
			}

			demande.GamerTag = gamerTag;

			Team team = uow.TeamRepository.GetByID(model.TeamID);
			if (team == null)
			{
				//TODO : Add client error
				return HttpNotFound();
			}
			demande.Team = team;

			uow.DemandeRepository.Insert(demande);
			uow.Save();

			return RedirectToAction("Details", new { id = model.TournamentID });
		}
	}
}