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

namespace LanAdeptAdmin.Views
{
	public class TournamentsController : Controller
	{
		UnitOfWork uow = UnitOfWork.Current;

		[Authorize]
		public ActionResult Index()
		{
			return View(uow.TournamentRepository.Get());
		}

		[Authorize]
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
		public ActionResult Create([Bind(Include = "TournamentID, GameID, StartTime")] Tournament tournament)
		{
			if (ModelState.IsValid)
			{
				tournament.CreationDate = DateTime.Now;
				uow.TournamentRepository.Insert(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}

			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournament.Game.GameID);
			return View(tournament);
		}

		[Authorize]
		public ActionResult Edit(int? id)
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
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournament.Game.GameID);
			return View(tournament);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "TournamentID, GameID, StartTime")] Tournament tournament)
		{
			if (ModelState.IsValid)
			{
				uow.TournamentRepository.Update(tournament);
				uow.Save();
				return RedirectToAction("Index");
			}
			ViewBag.GameID = new SelectList(uow.GameRepository.Get(), "GameID", "Name", tournament.Game.GameID);
			return View(tournament);
		}

		[Authorize]
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
			return View(tournament);
		}

		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Tournament tournament = uow.TournamentRepository.GetByID(id);
			uow.TournamentRepository.Delete(tournament);
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
