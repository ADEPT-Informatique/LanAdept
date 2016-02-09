using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using LanAdeptData.Model.Places;

namespace LanAdeptData.DAL.Places
{
	public class PlaceSectionRepository : GenericRepository<PlaceSection>
	{
		public PlaceSectionRepository(LanAdeptDataContext context) : base(context) { }
	}
}
