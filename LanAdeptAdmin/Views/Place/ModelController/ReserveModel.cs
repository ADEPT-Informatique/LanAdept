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

		public bool IsUser { get; set; }

		[DisplayName("Utilisateur inscrit")]
		public int UserID { get; set; }

		[DisplayName("Utilisateur non inscrit")]
		public string FullNameNoAccount { get; set; }
	}
}