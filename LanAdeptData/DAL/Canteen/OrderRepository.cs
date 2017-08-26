using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Canteen
{
    public class OrderRepository : GenericRepository<Order>
	{
		public OrderRepository(LanAdeptDataContext context) : base(context) { }
	}
}
