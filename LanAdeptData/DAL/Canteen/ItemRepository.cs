using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Canteen
{
    public class ItemRepository : GenericRepository<Item>
	{
		public ItemRepository(LanAdeptDataContext context) : base(context) { }
	}
}
