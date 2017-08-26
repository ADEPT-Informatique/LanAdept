using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdeptAdmin.Models
{
    public class PlaceSearchModel
	{
		[StringLength(int.MaxValue, MinimumLength=1)]
		public string Query { get; set; }

		public IEnumerable<Reservation> ReservationsFound { get; set; }

		public bool FoundReservation
		{
			get { return ReservationsFound != null && ReservationsFound.Count() > 0; }
		}

		public PlaceSearchModel()
		{
			ReservationsFound = new List<Reservation>();
		}
	}
}