using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Canteen
{
	public class Product
	{
		public int ProductID { get; set; }

		public string Name { get; set; }

		public decimal Price { get; set; }
		
		public bool InStock { get; set; }

		public bool IsVisible { get; set;}

		public string ImagePath { get; set; }
	}
}
