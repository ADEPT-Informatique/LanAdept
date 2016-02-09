using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdept.Views.Places.ModelController;
using LanAdeptCore.Service;
using LanAdeptCore.Service.ServiceResult;
using LanAdeptData.DAL;
using LanAdeptData.Model;
using System.IO;
using System.Web.UI;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.AspNet.Identity.Owin;
using LanAdeptCore.Attribute.Authorization;
using LanAdeptData.Model.Settings;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Maps;

namespace LanAdept.Controllers
{
	public class PlaceController : Controller
	{
		private const string ERROR_INVALID_ID = "Désolé, une erreur est survenue. Merci de réessayer dans quelques instants";
		private const string ERROR_RESERVE_LAN_STARTED = "Désolé, il est impossible de réserver une place lorsque le LAN est déjà commencé. Vous devrez vous présenter à l'accueil du LAN pour obtenir une place.";
		private const string ERROR_CANCEL_LAN_NO_RESERVATION = "Vous devez avoir une réservation pour pouvoir l'annuler.";
		private const string ERROR_CANCEL_LAN_STARTED = "Désolé, il est impossible d'annler une réservation lorsque le LAN est déjà terminé.";
		private const string ERROR_CANCEL_LAN_OVER = "Désolé, il est impossible d'annuler une réservation lorsque le LAN est déjà terminé.";

		private UnitOfWork uow
		{
			get { return UnitOfWork.Current; }
		}

		[AllowAnonymous]
		public ActionResult Index()
		{
			return RedirectToAction("Liste");
		}

		[AllowAnonymous]
		public ActionResult Liste()
		{
			ListeModel listeModel = new ListeModel();
			listeModel.Settings = uow.SettingRepository.GetCurrentSettings();

            List<FastMap> fastMaps = new List<FastMap>();
            foreach (Map map in uow.MapRepository.Get())
                fastMaps.Add(new FastMap(map));

            listeModel.Maps = fastMaps;

			if (!listeModel.Settings.IsLanStarted)
			{
				listeModel.NbPlacesLibres = uow.PlaceRepository.Get().Count(x => x.IsFree);
			}

			return View(listeModel);
		}

		[LanAuthorize]
		public ActionResult Reserver(int? id)
		{
			if (id == null || id < 1)
			{
				TempData["Error"] = ERROR_INVALID_ID;
				return RedirectToAction("Liste");
			}

			Setting settings = uow.SettingRepository.GetCurrentSettings();

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
				return RedirectToAction("Liste");
			}
			else
			{
				TempData["Success"] = "La place <strong>" + placeAReserver + "</strong> a bien été réservée.";
				return RedirectToAction("MaPlace");
			}
		}

		[LanAuthorize]
		public ActionResult MaPlace()
		{
			if (!ReservationService.HasUserPlace())
			{
				TempData["Error"] = "Vous n'avez pas encore réservé une place.";
				return RedirectToAction("Liste");
			}

			MaPlaceModel model = new MaPlaceModel();
			model.reservation = UserService.GetLoggedInUser().LastReservation;
			model.setting = uow.SettingRepository.GetCurrentSettings();

			return View(model);
		}

		[LanAuthorize]
		public ActionResult Annuler()
		{
			if (!ReservationService.HasUserPlace())
			{
				TempData["Error"] = ERROR_CANCEL_LAN_NO_RESERVATION;
				return RedirectToAction("Liste");
			}
			Setting settings = uow.SettingRepository.GetCurrentSettings();

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

		[LanAuthorize]
		public ActionResult GetBarcode(string id)
		{
			id = "(" + id + ")";

			Bitmap measureBmp = new Bitmap(1, 1);
			System.Drawing.Text.PrivateFontCollection fonts = new System.Drawing.Text.PrivateFontCollection();
			fonts.AddFontFile(HttpContext.Server.MapPath("~/fonts/code39.ttf"));
			Font font = new Font(fonts.Families[0], 24, FontStyle.Regular, GraphicsUnit.Pixel);
			SizeF imageSize;

			using (Graphics measureGraphics = Graphics.FromImage(measureBmp))
			{
				imageSize = measureGraphics.MeasureString(id, font);
			}

			Bitmap bmp = new Bitmap((int)imageSize.Width + 1, (int)imageSize.Height + 1, PixelFormat.Format32bppArgb);

			using (Graphics graph = Graphics.FromImage(bmp))
			{
				graph.CompositingQuality = CompositingQuality.HighQuality;
				graph.SmoothingMode = SmoothingMode.HighQuality;
				graph.InterpolationMode = InterpolationMode.High;
				graph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
				graph.Clear(Color.Transparent);

				graph.DrawString(id, font, Brushes.Black, 0, 0);
			}

			FileContentResult result;

			using (var memStream = new System.IO.MemoryStream())
			{
				bmp.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
				result = this.File(memStream.GetBuffer(), "image/png");
			}

			return result;
		}
	}
}