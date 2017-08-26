using System.Collections.Generic;
using System.Linq;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Places
{
    public class ReservationRepository : GenericRepository<Reservation>
	{
		public ReservationRepository(LanAdeptDataContext context) : base(context) { }

		public IEnumerable<Reservation> SearchByNameAndEmail(string query)
		{
			return Get(model =>
				(
					model.User == null && model.Guest.CompleteName.Contains(query)
				) ||
				(
					model.User != null &&
						(model.User.CompleteName.Contains(query) || model.User.Email.Contains(query))
				));
		}

		public Reservation GetByBarCode(string barcode)
		{
			return Get().Where(model => !model.IsGuest && model.User.Barcode == barcode).FirstOrDefault();
		}

	}
}
