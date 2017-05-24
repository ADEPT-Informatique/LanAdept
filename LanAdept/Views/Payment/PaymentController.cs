using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanAdept.Views.Paypal
{
   
    [LanAuthorize]
    public class PaymentController : Controller
    {
        private UnitOfWork uow
        {
            get { return UnitOfWork.Current; }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentWithPayPal(bool cancel)
        {
            throw new NotImplementedException();
        }
    }
}