using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Place.ModelController;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System.IO;
using System.Web.UI;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LanAdept.Controllers
{
    public class PlaceController : Controller
    {
        private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";
        private const string ERROR_RESERVE_LAN_STARTED = "Désolé, il est impossible de réserver une place lorsque le LAN est déjà commencé. Vous devrez vous présenter à l'accueil du LAN pour obtenir une place.";
        private const string ERROR_CANCEL_LAN_NO_RESERVATION = "Vous devez avoir une réservation pour pouvoir l'annuler.";
        private const string ERROR_CANCEL_LAN_STARTED = "Désolé, il est impossible d'annler une réservation lorsque le LAN est déjà terminé.";
        private const string ERROR_CANCEL_LAN_OVER = "Désolé, il est impossible d'annuler une réservation lorsque le LAN est déjà terminé.";

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
            ViewBag.Settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

            listeModel.Sections = uow.PlaceSectionRepository.Get();

            return View(listeModel);
        }

        [Authorize]
        public ActionResult Reserver(int? id)
        {
            if (id == null || id < 1)
            {
                TempData["Error"] = ERROR_INVALID_ID;
                return RedirectToAction("Liste");
            }

            Setting settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

            if (settings.IsLanStarted)
            {
                TempData["Error"] = ERROR_RESERVE_LAN_STARTED;
                return RedirectToAction("Liste");
            }

            Place placeAReserver = uow.PlaceRepository.GetByID(id.Value);

            if (placeAReserver == null)
            {
                TempData["Error"] = ERROR_INVALID_ID;
                return RedirectToAction("Liste");
            }

            BaseResult result = ReservationService.ReservePlace(placeAReserver);

            if (result.HasError)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = "La place <strong>" + placeAReserver + "</strong> a bien été réservée!";
            }

            return RedirectToAction("Liste");
        }

        [Authorize]
        public ActionResult MaPlace()
        {
            if (!ReservationService.HasUserPlace())
            {
                TempData["Error"] = "Vous n'avez pas encore réservé une place.";
                return RedirectToAction("Liste");
            }

            PDFModel model = new PDFModel();
            model.reservation = UserService.GetLoggedInUser().LastReservation;
            model.setting = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

            return View(model);
        }

        [Authorize]
        public ActionResult Annuler()
        {
            if (!ReservationService.HasUserPlace())
            {
                TempData["Error"] = ERROR_CANCEL_LAN_NO_RESERVATION;
                return RedirectToAction("Liste");
            }
            Setting settings = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();

            if (settings.IsLanOver)
            {
                TempData["Error"] = ERROR_CANCEL_LAN_OVER;
                return RedirectToAction("Liste");
            }
            else if (settings.IsLanStarted)
            {
                TempData["Error"] = ERROR_CANCEL_LAN_STARTED;
                return RedirectToAction("Liste");
            }

            ReservationService.CancelUserReservation();

            TempData["Success"] = "Votre réservation a été annulée.";
            return RedirectToAction("Liste");
        }

        [Authorize]
        public ActionResult CreateTicket(Reservation reservation, Setting setting)
        {
            PDFModel model = new PDFModel();
            model.reservation = UserService.GetLoggedInUser().LastReservation;
            model.setting = LanAdeptData.DAL.UnitOfWork.Current.SettingRepository.GetCurrentSettings();
            string html = RenderViewToString(ControllerContext, "~/Views/Place/DownloadFilePDFPartiel.cshtml", model, true);
            string css = System.IO.File.ReadAllText(Server.MapPath("~/Content/PDF.css"));
            CreateFilePDF(html, css);
            return View("MaPlace");
        }

        private void CreateFilePDF(string html, string css)
        {
            Byte[] bytes;

            using (var ms = new MemoryStream())
            {

                using (var doc = new Document())
                {

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {

                        doc.Open();

                        using (var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss);
                            }
                        }


                        doc.Close();
                    }
                }
                bytes = ms.ToArray();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=Ticket.pdf");
            Response.ContentType = "application/pdf";
            Response.Flush();
            Response.BinaryWrite(bytes);
            Response.End();
        }

        static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }
    }
}