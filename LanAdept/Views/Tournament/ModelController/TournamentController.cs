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
                tournamentModel.GamerTags = uow.GamerTagRepository.GetGamerTagsByUser(user);
            }

            return View(tournamentModel);
		}

		[AllowAnonymous]
		public ActionResult Addteam(int id)
		{
			TeamModel team = new TeamModel();
			team.Tournament = uow.TournamentRepository.GetByID(id);
			team.TeamLeader = UserService.GetLoggedInUser();
            team.TournamentID = team.Tournament.TournamentID;
            team.UserID = team.TeamLeader.UserID;
			return View(team);
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Addteam([Bind(Include = "Name,Tag,TournamentID,UserID")]TeamModel teamModel)
		{
			if (ModelState.IsValid)
			{
				Team team = new Team();
				team.Name = teamModel.Name;
				team.Tag = teamModel.Tag;
				team.TeamLeader = uow.UserRepository.GetByID(teamModel.UserID);
				
				List<User> users = new List<User>();
                users.Add(uow.UserRepository.GetByID(teamModel.UserID));
				team.Users = users;
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

			return View(teamModel);
		}

        [Authorize]
        public ActionResult AddGamerTag(GamerTagModel model)
        {
            User user = UserService.GetLoggedInUser();

            if (!uow.GamerTagRepository.HasSameGamerTag(model.Gamertag))
            {
                GamerTag gamerTag = new GamerTag();
                gamerTag.Gamertag = model.Gamertag;
                gamerTag.User = user;

                uow.GamerTagRepository.Insert(gamerTag);
                uow.Save();   
            }

            return RedirectToAction("Details", new { id = model.TournamentID });
        }

        [Authorize]
        public ActionResult JoinTeam(JoinTeamModel model)
        {
            if (model.GamerTagID == null || model.TournamentID == null || model.TeamID == null )
            {
                return HttpNotFound();
            }

            Demande demande = new Demande();
            User user = UserService.GetLoggedInUser();

            GamerTag gamerTag = uow.GamerTagRepository.GetGamerTagByUserAndGamerTagID(user, model.GamerTagID.Value);
            if (gamerTag == null)
            {
                return HttpNotFound();
            }

            demande.GamerTag = gamerTag;

            Team team = uow.TeamRepository.GetByID(model.TeamID);
            if (team == null)
            {
                return HttpNotFound();
            }
            demande.Team = team;

            uow.DemandeRepository.Insert(demande);
            uow.Save();

            return RedirectToAction("Details", new { id = model.TournamentID });
        }
	}
}