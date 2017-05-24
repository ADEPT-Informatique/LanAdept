using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Settings
{
	public class Setting
	{
		public int SettingId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public DateTime PlaceReservationStartDate { get; set; }

		public DateTime TournamentSubsciptionStartDate { get; set; }

		public string Rules { get; set; }

        public string Description { get; set; }

		public bool SendRememberEmail { get; set; }

		public int NbDaysBeforeRemember { get; set; }

		public string RememberEmailContent { get; set; }
        public string EventKeyId { get; set; }
        public string PublicKeyId { get; set; }
        public string SecretKeyId { get; set; }

        public bool IsPaypalActive { get; set; }
        public string PaypalClientId { get; set; }
        public string PaypalSecretId { get; set; }
        public double TicketPrice { get; set; }

		#region Calculated Properties

		public bool IsLanStarted { get { return DateTime.Now > StartDate; } }

		public bool IsLanOver { get { return DateTime.Now > EndDate; } }

		public bool IsPlaceReservationStarted { get { return DateTime.Now > PlaceReservationStartDate; } }

		public bool TournamentSubsciptionStarted { get { return DateTime.Now > TournamentSubsciptionStartDate; } }

		#endregion
	}
}
