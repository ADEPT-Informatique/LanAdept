using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Reservation
	{
		public int ReservationID { get; set; }

		public bool EstAnnule { get; set; }

		public DateTime DateCreation { get; set; }

		public DateTime? DateArrive { get; set; }

		public DateTime? DateDepart { get; set; }

		#region Navigation properties

		public virtual Place Place { get; set; }

		public virtual User User { get; set; }

		#endregion

		#region Calculated properties

		public bool EstOccupe
		{
			get { return DateArrive != null && DateDepart == null; }
		}

		#endregion
	}
}
