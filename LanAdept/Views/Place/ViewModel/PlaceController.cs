using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Place.ViewModel;
using LanAdeptData.DAL;

namespace LanAdept.Controllers
{
	[Authorize]
	public class PlaceController : Controller
	{
		private UnitOfWork uow = UnitOfWork.Current;

		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();

			listeModel.Sections = uow.PlaceSectionRepository.Get();

			return View(listeModel);
		}
	}
}