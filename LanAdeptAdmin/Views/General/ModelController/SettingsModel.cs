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
        [DisplayName("Clé public pour les places")]
        public string PublicId { get; set; }
        [DisplayName("Clé secrete pour les places")]
        public string SecretId { get; set; }
        [DisplayName("Clé evenement pour les places")]
        public string EventId { get; set; }
        [DisplayName("Clé de client paypal pour le paiement en ligne")]
        public string PaypalClientId { get; set; }
        [DisplayName("Clé secrete de paypal pour le paiement en ligne")]
        public string PaypalSecretId { get; set; }
        [DisplayName("Paiement par paypal activé")]
        public bool IsPaypalActive { get; set; }
        [DisplayName("Prix du billet")]
        public double TicketPrice { get; set; }

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
            PublicId = settings.PublicKeyId;
            EventId = settings.EventKeyId;
            SecretId = settings.SecretKeyId;
            IsPaypalActive = settings.IsPaypalActive;
            PaypalClientId = settings.PaypalClientId;
            PaypalSecretId = settings.PaypalSecretId;
        
		}

	}
}