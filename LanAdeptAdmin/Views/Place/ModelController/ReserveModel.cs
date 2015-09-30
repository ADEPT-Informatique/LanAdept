using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Views.Places.ModelController
{
	public class ReserveModel
	{
		public Place Place { get; set; }

		public int PlaceID { get; set; }

		[DisplayName("Réserver pour un utilisateur")]
		public bool IsUser { get; set; }

		public int UserID { get; set; }

		public string FullNameNoAccount { get; set; }
	}
}