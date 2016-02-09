using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using LanAdeptData.Model.Maps;

namespace LanAdeptData.DAL.Maps
{
	public class TileRepository : GenericRepository<Tile>
    {
		public TileRepository(LanAdeptDataContext context) : base(context) { }
    }
}
