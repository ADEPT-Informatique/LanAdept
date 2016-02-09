using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanAdeptData.Model;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Maps;

namespace LanAdeptAdmin.Views.Places.ModelController
{
	public class ListeModel
	{
        public IEnumerable<Map> Maps { get; set; }
		public IEnumerable<PlaceSection> Sections { get; set; }

	}
}