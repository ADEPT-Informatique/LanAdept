using LanAdeptData.DAL.Generic;
using LanAdeptData.Model.Canteen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL.Canteen
{
	public class ItemRepository : GenericRepository<Item>
	{
		public ItemRepository(LanAdeptDataContext context) : base(context) { }
	}
}
