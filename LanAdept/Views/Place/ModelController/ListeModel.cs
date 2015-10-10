using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdept.Views.Place.ModelController
{
	public class ListeModel
	{
        public IEnumerable<Map> Maps { get; set; }
		public IEnumerable<PlaceSection> Sections { get; set; }
		public int NbPlacesLibres { get; set; }
		public Setting Settings { get; set; }

	}
}