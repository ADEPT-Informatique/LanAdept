using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.DAL;
using LanAdeptData.Model;

namespace LanAdept.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        UnitOfWork uow = new UnitOfWork();

        public ActionResult Index()
        {
            DateTime dateLan = new DateTime(2015, 10, 18, 12, 0, 0);

            ViewBag.dateLan = dateLan.Date.ToString("s");
            return View();
        }
    }
}