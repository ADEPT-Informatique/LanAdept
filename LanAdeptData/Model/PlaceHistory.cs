using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class PlaceHistory
	{
		public int PlaceHistoryID { get; set; }

		public DateTime Date { get; set; }

		public string Type { get; set; }

		public virtual Place Place { get; set; }

		public virtual User User { get; set; }
	}
}
