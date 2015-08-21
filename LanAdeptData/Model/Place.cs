using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Place
	{
		public int PlaceID { get; set; }

		public int Number { get; set; }

		[ForeignKey("PlaceSection")]
		public virtual int PlaceSectionID { get; set; }

		public virtual PlaceSection PlaceSection { get; set; }

		public virtual ICollection<PlaceHistory> PlaceHistories { get; set; }
	}
}
