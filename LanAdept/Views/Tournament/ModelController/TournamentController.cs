using LanAdeptData.DAL;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using LanAdeptData.Model;
using LanAdeptCore.Service;
using System.Data;

namespace LanAdept.Views.Tournament.ModelController
{
	public class TournamentController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[AllowAnonymous]
		public ActionResult Index()
		{
			List<TournamentModel> tournamentModels = new List<TournamentModel>();
			IEnumerable<LanAdeptData.Model.Tournament> tournaments = uow.TournamentRepository.Get();
			foreach (LanAdeptData.Model.Tournament tournament in tournaments)
			{
				tournamentModels.Add(new TournamentModel(tournament));
			}
			return View(tournamentModels);
		}

		[AllowAnonymous]
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

            LanAdeptData.Model.Tournament tournament = uow.TournamentRepository.GetByID(id);

            if (tournament == null)
            {
                return HttpNotFound();
            }

            TournamentModel tournamentModel = new TournamentModel(tournament);

            User user = UserService.GetLoggedInUser();
            if (user != null)
            {
                tournamentModel.IsConnected = true;
                tournamentModel.GamerTags = uow.GamerTagRepository.GetByUser(user);
                tournamentModel.UserTeam = uow.TeamRepository.UserTeamInTournament(user, tournament);
            }

            return View(tournamentModel);
		}

        [Authorize]
        public ActionResult Addteam(int id)
        {
            AddTeamModel team = new AddTeamModel();
            team.Tournament = uow.TournamentRepository.GetByID(id);
            team.TournamentID = team.Tournament.TournamentID;
            team.GamerTags = uow.GamerTagRepository.GetByUser(UserService.GetLoggedInUser());
            return View(team);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Addteam(AddTeamModel teamModel)
        {
            if (ModelState.IsValid)
            {
                Team team = new Team();
                team.Name = teamModel.Name;
                team.Tag = teamModel.Tag;
                User user = UserService.GetLoggedInUser();
                team.TeamLeaderTag = uow.GamerTagRepository.GetByUserAndGamerTagID(user, teamModel.GamerTagId);

                List<GamerTag> listGamerTags = new List<GamerTag>();
                listGamerTags.Add(team.TeamLeaderTag);
                team.GamerTags = listGamerTags;
                team.Tournament = uow.TournamentRepository.GetByID(teamModel.TournamentID);

                uow.TeamRepository.Insert(team);

                LanAdeptData.Model.Tournament tournament = team.Tournament;

                if (tournament.Teams == null)
                {
                    ICollection<Team> teamList;
                    teamList = new List<Team>();
                    teamList.Add(team);
                    tournament.Teams = teamList;
                }
                else
                {
                    tournament.Teams.Add(team);
                }

                uow.TournamentRepository.Update(tournament);

                uow.Save();
                return RedirectToAction("Details", new { id = team.Tournament.TournamentID });
            }

            //IEnumerable<GamerTag> gamerTags = uow.GamerTagRepository.GetByUser(UserService.GetLoggedInUser());
            //teamModel.LeaderTags = new SelectList(gamerTags, "GamerTagID", "Gamertag");

            teamModel.GamerTags = uow.GamerTagRepository.GetByUser(UserService.GetLoggedInUser());
            return View(teamModel);
        }

        [Authorize]
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

        [Authorize]
        public ActionResult JoinTeam(JoinTeamModel model)
        {
            if (model.GamerTagID == null || model.TournamentID == null || model.TeamID == null )
            {
                //TODO : Add client error
                return HttpNotFound();
            }

            Demande demande = new Demande();
            User user = UserService.GetLoggedInUser();

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

		#region Team Management
		[Authorize]
		public ActionResult DetailsTeam(int? teamId)
		{
			if (teamId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			DetailsTeamModel team = new DetailsTeamModel(uow.TeamRepository.GetByID(teamId));

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
		#endregion
	}
}