using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using LanAdeptData.Model;
using LanAdeptData.Model.Settings;
using System.ComponentModel.DataAnnotations;

namespace LanAdeptAdmin.Views.General.ModelController
{
	public class SettingsModel
	{
		public int SettingId { get; set; }

		[DisplayName("Date de début")]
		[Required]
		public DateTime StartDate { get; set; }

		[DisplayName("Date de fin")]
		[Required]
		public DateTime EndDate { get; set; }

		[DisplayName("Date d'ouverture des réservations")]
		[Required]
		public DateTime PlaceReservationStartDate { get; set; }

		[DisplayName("Date d'ouverture des inscriptions aux tournois")]
		[Required]
		public DateTime TournamentSubsciptionStartDate { get; set; }

		[DisplayName("Envoyer un rappel du début du Lan")]
		public bool SendRememberEmail { get; set; }

		[DisplayName("Nombre de jour avant le rappel")]
		public int NbDaysBeforeRemember { get; set; }


		public SettingsModel() { }

		public SettingsModel(Setting settings)
		{
			SettingId = settings.SettingId;
			StartDate = settings.StartDate;
			EndDate = settings.EndDate;
			PlaceReservationStartDate = settings.PlaceReservationStartDate;
			TournamentSubsciptionStartDate = settings.TournamentSubsciptionStartDate;
			SendRememberEmail = settings.SendRememberEmail;
			NbDaysBeforeRemember = settings.NbDaysBeforeRemember;
		}

	}
}