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
            DateTime dateLan = new DateTime(2015, 10, 14, 12, 0, 0);

            double dateLanMs = dateLan.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            ViewBag.dateLan = dateLanMs;
            return View();
        }
    }
}