using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Canteen
{
	public class Item
	{
		public int ItemID { get; set; }

		public int Quantity { get; set; }

		#region Navigation properties

		public virtual Product Product { get; set; }

		#endregion

		#region Calculated properties

		public decimal Price { get { return Quantity * Product.Price; } }

		#endregion

	}
}
