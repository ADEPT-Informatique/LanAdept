using System;
using System.Collections.Generic;

namespace LanAdeptData.Model
{
    public class Order
	{
		public int OrderID { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? CancellationDate { get; set; }

		public DateTime? DeliveryDate { get; set; }

		#region Navigation properties

		public virtual User User { get; set; }

		public virtual List<Item> Items { get; set; }

		#endregion

		#region Calculated properties

		public bool IsCancelled { get { return CancellationDate != null; } }

		public bool IsDelivered { get { return CancellationDate != null; } }

		public decimal Price
		{
			get
			{
				decimal result = 0m;
				foreach (Item item in Items)
				{
					result += item.Price;
				}
				return result;
			}
		}

		#endregion

	}
}
