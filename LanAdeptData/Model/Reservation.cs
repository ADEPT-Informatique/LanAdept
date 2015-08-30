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

		public DateTime CreationDate { get; set; }

		public DateTime? ArrivalDate { get; set; }

		public DateTime? LeavingDate { get; set; }

		public DateTime? CancellationDate { get; set; }

		#region Navigation properties

		public virtual Place Place { get; set; }

		public virtual User User { get; set; }

		#endregion

		#region Calculated properties

		public bool IsCancelled
		{
			get { return CancellationDate != null; }
		}

		#endregion
	}
}
