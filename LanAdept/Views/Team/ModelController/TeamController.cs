using LanAdept.Views.Tournaments.ModelController;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptCore.Service;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Teams.ModelController;

namespace LanAdept.Controllers
{
	public class TeamController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			int TeamLeaderID = UserService.GetLoggedInUser().UserID;
			IEnumerable<Team> teams = uow.TeamRepository.GetByTeamLeaderID(TeamLeaderID);

			IndexTeamModel model = new IndexTeamModel();
			model.Teams = new List<TeamDemandeModel>();

			foreach (Team team in teams)
			{
				TeamDemandeModel tdm = new TeamDemandeModel();

				tdm.Team = team;

				tdm.Demandes = new List<Demande>();

				foreach (Demande demande in uow.DemandeRepository.Get())
				{
					if (demande.Team.TeamID == team.TeamID)
					{
						tdm.Demandes.Add(demande);
					}
				}

				model.Teams.Add(tdm);
			}

			return View(model);
		}

		[AuthorizePermission("user.tournament.team.details")]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			DetailsModel teamModel = new DetailsModel();

			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.UserID != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			teamModel.GamerTags = team.GamerTags;
			teamModel.TeamID = team.TeamID;
			teamModel.TournamentID = team.TournamentID;
			teamModel.Tournament = team.Tournament;
			teamModel.MaxPlayerPerTeam = team.Tournament.MaxPlayerPerTeam;
			teamModel.Name = team.Name;
			teamModel.Tag = team.Tag;
			teamModel.TeamLeaderTag = team.TeamLeaderTag;

			List<Demande> demandes = new List<Demande>();

			foreach (Demande demande in uow.DemandeRepository.Get())
			{
				if (demande.Team.TeamID == team.TeamID)
				{
					demandes.Add(demande);
				}
			}

			teamModel.Demandes = demandes;


			if (teamModel == null)
			{
				return HttpNotFound();
			}

			return View(teamModel);
		}

		[AuthorizePermission("user.tournament.team.kick")]
		public ActionResult KickPlayer(int? id, int? gamerTagId)
		{
			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.UserID != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			if (team.TeamLeaderTag == gamerTag)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas kicker le team leader.";
				return RedirectToAction("Details", new { id = id });
			}
			else
			{
				team.GamerTags.Remove(gamerTag);

				uow.TeamRepository.Update(team);
				uow.GamerTagRepository.Update(gamerTag);
				uow.Save();
			}

			return RedirectToAction("Details", new { id = id });
		}

		[Authorize]
		public ActionResult AcceptTeamMember(int id, int gamerTagId)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.UserID != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			if (team.GamerTags.Count < team.Tournament.MaxPlayerPerTeam)
			{
				GamerTag gamer = uow.GamerTagRepository.GetByID(gamerTagId);

				team.GamerTags.Add(gamer);

				uow.TeamRepository.Update(team);

				List<Demande> demandes = uow.DemandeRepository.GetByGamerTagId(gamerTagId);

				foreach (Demande demande in demandes)
				{
					if (demande.Team.Tournament.TournamentID == team.Tournament.TournamentID)
					{
						uow.DemandeRepository.Delete(demande);
					}
				}

				uow.Save();
			}
			return RedirectToAction("Details", new { id = id });
		}

		[Authorize]
		public ActionResult RefuseTeamMember(int id, int gamerTagId)
		{
			GamerTag gamer = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.UserID != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			List<Demande> demandes = uow.DemandeRepository.GetByGamerTagId(gamerTagId);

			foreach (Demande demande in demandes)
			{
				if (demande.Team.TeamID == id && user.UserID == team.TeamLeaderTag.UserID)
				{
					uow.DemandeRepository.Delete(demande);
				}
			}

			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[Authorize]
		public ActionResult LeaveTeam(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);
			User user = UserService.GetLoggedInUser();

			bool IsGamer = false;

			foreach (GamerTag gamer in team.GamerTags)
			{
				if (gamer.UserID == user.UserID)
				{
					IsGamer = true;
					break;
				}
			}

			if (IsGamer)
			{
				GamerTag tag = team.GamerTags.First(g => g.UserID == user.UserID);

				team.GamerTags.Remove(tag);
				uow.TeamRepository.Update(team);
				uow.Save();
			}

			return RedirectToAction("Details", "Tournament", new { id = team.Tournament.TournamentID });
		}

		[Authorize]
		public ActionResult CancelDemande(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();

			foreach (Demande demande in team.Demandes)
			{
				if (demande.GamerTag.UserID == user.UserID)
				{
					uow.DemandeRepository.Delete(demande);
					break;
				}
			}

			uow.Save();

			return RedirectToAction("Details", "Tournament", new { id = team.Tournament.TournamentID });
		}

		[Authorize]
		public ActionResult Delete(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.UserID != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			List<Demande> demandes = uow.DemandeRepository.GetByTeamId(id);
			foreach (Demande demande in demandes)
			{
				uow.DemandeRepository.Delete(demande);
			}
			int tournamentID = team.Tournament.TournamentID;

			uow.TeamRepository.Delete(team);

			uow.Save();

			return RedirectToAction("Details", "Tournament", new { id = tournamentID });
		}
	}
}