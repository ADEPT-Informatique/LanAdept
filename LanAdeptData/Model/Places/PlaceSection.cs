using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Places
{
	public class PlaceSection
	{
		public int PlaceSectionID { get; set; }

		public string Name { get; set; }

		public virtual ICollection<Place> Places { get; set; }
	}
}
