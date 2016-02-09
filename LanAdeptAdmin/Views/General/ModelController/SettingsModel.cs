using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using LanAdeptData.Model;
using LanAdeptData.Model.Settings;

namespace LanAdeptAdmin.Views.General.ModelController
{
	public class SettingsModel
	{
		public int SettingId { get; set; }

		[DisplayName("Date de début")]
		public DateTime StartDate { get; set; }

		[DisplayName("Date de fin")]
		public DateTime EndDate { get; set; }
		 
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
			SendRememberEmail = settings.SendRememberEmail;
			NbDaysBeforeRemember = settings.NbDaysBeforeRemember;
		}

	}
}