namespace LanAdeptData.Model
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
