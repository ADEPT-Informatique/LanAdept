using LanAdept.Views.Tournament.ModelController;
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

namespace LanAdept.Views.Team.ModelController
{
	public class TeamController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			int TeamLeaderID = LanAdeptCore.Service.UserService.GetLoggedInUser().UserID;
			IEnumerable<LanAdeptData.Model.Team> teams = uow.TeamRepository.GetByTeamLeaderID(TeamLeaderID);

			IndexTeamModel model = new IndexTeamModel();
			model.Teams = new List<TeamDemandeModel>();

			foreach (LanAdeptData.Model.Team team in teams)
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
		public ActionResult DetailsTeam(int? teamId)
		{
			if (teamId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			DetailsTeamModel teamModel = new DetailsTeamModel();

			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			if (team.TeamLeaderTag.User != UserService.GetLoggedInUser())
			{
				return RedirectToAction("Index","Home");
			}

			teamModel.GamerTags = team.GamerTags;
			teamModel.TeamID = team.TeamID;
			teamModel.TournamentID = team.TournamentID;
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
		public ActionResult KickPlayer(int? gamerTagId, int? teamId)
		{
			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			if (team.TeamLeaderTag == gamerTag || team.GamerTags.Count == 1)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas kicker le team leader.";
				return RedirectToAction("DetailsTeam", new { teamId = teamId });
			}
			else
			{
				team.GamerTags.Remove(gamerTag);

				uow.TeamRepository.Update(team);
				uow.GamerTagRepository.Update(gamerTag);
				uow.Save();
			}

			return RedirectToAction("DetailsTeam", new { teamId = teamId });
		}

		[Authorize]
		public ActionResult AcceptTeamMember(int gamerTagId, int teamId)
		{
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);
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
			return RedirectToAction("DetailsTeam", new { teamId = teamId });
		}

		[Authorize]
		public ActionResult RefuseTeamMember(int gamerTagId, int teamId)
		{
			GamerTag gamer = uow.GamerTagRepository.GetByID(gamerTagId);
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			List<Demande> demandes = uow.DemandeRepository.GetByGamerTagId(gamerTagId);

			foreach (Demande demande in demandes)
			{
				if (demande.Team.TeamID == teamId)
				{
					uow.DemandeRepository.Delete(demande);
				}
			}

			uow.Save();

			return RedirectToAction("DetailsTeam", new { teamId = teamId });
		}

		//TODO: LeaveTeam
		[Authorize]
		public ActionResult LeaveTeam(int teamId)
		{
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			foreach (GamerTag gamer in team.GamerTags)
			{
				if (gamer.UserID == UserService.GetLoggedInUser().UserID)
				{
					team.GamerTags.Remove(gamer);

					uow.TeamRepository.Update(team);
					uow.GamerTagRepository.Update(gamer);
					uow.Save();
				}
			}

			return RedirectToAction("Details","Tournament", new { id = team.Tournament.TournamentID });
		}

		//TODO: CancelDemande
		[Authorize]
		public ActionResult CancelDemande(int teamId)
		{
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			List<Demande> demandes = uow.DemandeRepository.GetByTeamId(teamId);
            foreach (Demande demande in demandes)
			{
				if (demande.GamerTag.UserID == UserService.GetLoggedInUser().UserID)
				{
					uow.DemandeRepository.Delete(demande);
				}
			}

			uow.Save();


			return RedirectToAction("Details", "Tournament", new { id = team.Tournament.TournamentID });
		}

		//TODO: DeleteTeam
		[Authorize]
		public ActionResult DeleteTeam(int teamId)
		{
			LanAdeptData.Model.Team team = uow.TeamRepository.GetByID(teamId);

			List<Demande> demandes = uow.DemandeRepository.GetByTeamId(teamId);
			foreach (Demande demande in demandes)
			{
				uow.DemandeRepository.Delete(demande);
			}

			uow.TeamRepository.Delete(team);

			uow.Save();

			return RedirectToAction("Index", "Home");
		}
	}
}