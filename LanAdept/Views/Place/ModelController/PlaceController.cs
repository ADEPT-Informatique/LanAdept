using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Place.ModelController;
using LanAdeptData.DAL;

namespace LanAdept.Controllers
{
	public class PlaceController : Controller
	{
		private UnitOfWork uow = UnitOfWork.Current;

		[AllowAnonymous]
		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		[AllowAnonymous]
		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();

			listeModel.Sections = uow.PlaceSectionRepository.Get();

			return View(listeModel);
		}

		[Authorize]
		public ActionResult Place(int? id)
		{
			if (id == null)
				return RedirectToAction("Liste");



			return View();
		}
	}
}