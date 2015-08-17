using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Places
{
	public class PlaceRepository : GenericRepository<Place>
	{
		public PlaceRepository(LanAdeptDataContext context) : base(context) { }
	}
}
