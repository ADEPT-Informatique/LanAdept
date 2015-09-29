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

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
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

		[Authorize]
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

		[Authorize]
		public ActionResult Create()
		{
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name");
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "GameID, StartTime")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = new Tournament();

				tournament.StartTime = tournamentModel.StartTime;
				tournament.GameID = tournamentModel.GameID;

				tournament.CreationDate = DateTime.Now;

				uow.TournamentRepository.Insert(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournamentModel.GameID);
			return View(tournamentModel);
		}

		[Authorize]
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
			ViewBag.Id = tournament.Id;
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournament.Game.GameID);
			return View(tournament);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "GameID, StartTime, Id")] TournamentModel tournamentModel)
		{
			if (ModelState.IsValid)
			{
				Tournament tournament = uow.TournamentRepository.GetByID(tournamentModel.Id);

				tournament.Game = tournamentModel.Game;
				tournament.StartTime = tournamentModel.StartTime;

				uow.TournamentRepository.Update(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournamentModel.Game.GameID);
			return View(tournamentModel);
		}

		[Authorize]
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

		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			uow.TournamentRepository.Delete(uow.TournamentRepository.GetByID(id));
			uow.Save();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				uow.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
