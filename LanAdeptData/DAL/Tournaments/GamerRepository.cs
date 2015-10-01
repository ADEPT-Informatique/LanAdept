using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Tournaments
{
	public class GamerRepository : GenericRepository<Gamer>
	{
		public GamerRepository(LanAdeptDataContext context) : base(context) { }
	}
}

