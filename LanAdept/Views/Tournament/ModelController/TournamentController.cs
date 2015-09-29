using LanAdeptData.DAL;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using LanAdeptData.Model;
using LanAdeptCore.Service;

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
			team.TournamentID = id;
			User user = UserService.GetLoggedInUser();
			int userId = user.UserID;
            team.UserID = userId;
			return View(team);
		}

		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Addteam([Bind(Include = "Name,TournamentID,UserID")] TeamModel teamModel)
		{
			if (ModelState.IsValid)
			{
				Team team = new Team();

				team.TeamID = teamModel.Id;
				team.Name = teamModel.Name;
				team.UserID = teamModel.UserID;
				team.TeamLeader = uow.UserRepository.GetByID(teamModel.UserID);
				List<User> users = new List<User>();
				users.Add(uow.UserRepository.GetByID(teamModel.UserID));
				team.Users = users;
				team.TournamentID = teamModel.TournamentID;

				uow.TeamRepository.Insert(team);

				uow.TournamentRepository.GetByID(team.TournamentID).Teams.Add(team);

				uow.Save();
				return RedirectToAction("Details", new { id = team.TournamentID } );
			}

			return View(teamModel);
		}
	}
}