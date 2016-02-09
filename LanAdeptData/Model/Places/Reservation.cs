using LanAdeptData.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Places
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

		public virtual Guest Guest { get; set; }

		#endregion

		#region Calculated properties

		public bool IsGuest
		{
			get { return Guest != null; }
		}

		public bool IsCancelled
		{
			get { return CancellationDate != null; }
		}

		public bool HasArrived
		{
			get { return ArrivalDate != null; }
		}

		public bool HasLeft
		{
			get { return LeavingDate != null; }
		}

		public string UserCompleteName
		{
			get {
				if (IsGuest)
					return Guest.CompleteName;

				return User.CompleteName;
			}
		}

		#endregion
	}
}
