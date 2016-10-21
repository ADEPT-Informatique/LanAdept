using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanAdeptData.Model;
using LanAdeptData.Model.Places;

namespace LanAdeptAdmin.Views.Places.ModelController
{
	public class ReserveModel
	{
		public Place Place { get; set; }

		public string SeatsId { get; set; }

		public bool IsGuest { get; set; }

		[DisplayName("Utilisateur")]
		public string UserID { get; set; }

		public IEnumerable<SelectListItem> Users { get; set; } 

		[StringLength(50, MinimumLength = 4)]
		[DisplayName("Nom complet")]
		public string FullNameNoAccount { get; set; }
	}
}