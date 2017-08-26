using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Canteen
{
    public class ProductRepository : GenericRepository<Product>
	{
		public ProductRepository(LanAdeptDataContext context) : base(context) { }
	}
}
