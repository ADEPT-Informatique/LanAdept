using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Places
{
	public class Place
	{
        public int PlaceID { get; set; }
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string SeatsId { get; set; }
        public bool IsBackUpSeats { get; set; }

		#region Navigation properties


		public virtual ICollection<Reservation> Reservations { get; set; }

		#endregion

		#region Calculated properties

		public Reservation LastReservation
		{
			get
			{
				return Reservations.OrderBy(x => x.ReservationID).LastOrDefault();
			}
		}

		/// <summary>
		/// Determine wether the place is currently available and free or not
		/// </summary>
		public bool IsFree
		{
			get
			{
				return LastReservation == null
					|| LastReservation.IsCancelled == true
					|| LastReservation.LeavingDate != null;
			}
		}

		/// <summary>
		/// Determine whether the place is reserved or not
		/// </summary>
		public bool IsReserved
		{
			get
			{
				return LastReservation != null
					&& LastReservation.IsCancelled == false
					&& LastReservation.ArrivalDate == null;
			}
		}

		/// <summary>
		/// Determined wether there is actually someone sitting at this place or not
		/// </summary>
		public bool IsOccupied
		{
			get
			{
				return LastReservation != null
					&& LastReservation.ArrivalDate != null
					&& LastReservation.LeavingDate == null;
			}
		}


		#endregion

		public override string ToString()
		{
            return PlaceID.ToString();
		}
	}
}
