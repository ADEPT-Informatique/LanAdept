﻿using LanAdept.Views.Tournaments.ModelController;
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
using Microsoft.AspNet.Identity.Owin;

namespace LanAdept.Controllers
{
	public class TeamController : Controller
	{
		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[LanAuthorize]
		public ActionResult Index()
		{
			string TeamLeaderID = UserService.GetLoggedInUser().Id;
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

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			DetailsModel teamModel = new DetailsModel();

			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.Id != team.TeamLeaderTag.UserID)
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

		public ActionResult KickPlayer(int? id, int? gamerTagId)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}

			Team team = uow.TeamRepository.GetByID(id);
			if (team == null)
			{
				return RedirectToAction("Index");
			}

			if (gamerTagId == null)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas kicker le team leader.";
				return RedirectToAction("Details", new { id = team.TeamID });
			}

			GamerTag gamerTag = uow.GamerTagRepository.GetByID(gamerTagId);

			User user = UserService.GetLoggedInUser();
			if (user.Id != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			if (team.TeamLeaderTag == gamerTag)
			{
				TempData["ErrorMessage"] = "Vous ne pouvez pas retirer le team leader.";
				return RedirectToAction("Details", new { id = id });
			}

			team.GamerTags.Remove(gamerTag);
			uow.TeamRepository.Update(team);
			uow.GamerTagRepository.Update(gamerTag);
			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize]
		public ActionResult AcceptTeamMember(int id, int gamerTagId)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.Id != team.TeamLeaderTag.UserID)
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

		[LanAuthorize]
		public ActionResult RefuseTeamMember(int id, int gamerTagId)
		{
			GamerTag gamer = uow.GamerTagRepository.GetByID(gamerTagId);
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.Id != team.TeamLeaderTag.UserID)
			{
				return RedirectToAction("Index", "Home");
			}

			List<Demande> demandes = uow.DemandeRepository.GetByGamerTagId(gamerTagId);

			foreach (Demande demande in demandes)
			{
				if (demande.Team.TeamID == id && user.Id == team.TeamLeaderTag.UserID)
				{
					uow.DemandeRepository.Delete(demande);
				}
			}

			uow.Save();

			return RedirectToAction("Details", new { id = id });
		}

		[LanAuthorize]
		public ActionResult LeaveTeam(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);
			User user = UserService.GetLoggedInUser();

			bool IsGamer = false;

			foreach (GamerTag gamer in team.GamerTags)
			{
				if (gamer.UserID == user.Id)
				{
					IsGamer = true;
					break;
				}
			}

			if (IsGamer)
			{
				GamerTag tag = team.GamerTags.First(g => g.UserID == user.Id);

				team.GamerTags.Remove(tag);
				uow.TeamRepository.Update(team);
				uow.Save();
			}

			return RedirectToAction("Details", "Tournament", new { id = team.Tournament.TournamentID });
		}

		[LanAuthorize]
		public ActionResult CancelDemande(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();

			foreach (Demande demande in team.Demandes)
			{
				if (demande.GamerTag.UserID == user.Id)
				{
					uow.DemandeRepository.Delete(demande);
					break;
				}
			}

			uow.Save();

			return RedirectToAction("Details", "Tournament", new { id = team.Tournament.TournamentID });
		}

		[LanAuthorize]
		public ActionResult Delete(int id)
		{
			Team team = uow.TeamRepository.GetByID(id);

			User user = UserService.GetLoggedInUser();
			if (user.Id != team.TeamLeaderTag.UserID)
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