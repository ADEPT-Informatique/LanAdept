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
			TournamentModel tournament = new TournamentModel(uow.TournamentRepository.GetByID(id));
			if (tournament == null)
			{
				return HttpNotFound();
			}
			return View(tournament);
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

        //        @Html.HiddenFor(model => model.Tournament.TournamentID)
        //@Html.HiddenFor(model => model.TeamLeader.UserID)

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
	}
}