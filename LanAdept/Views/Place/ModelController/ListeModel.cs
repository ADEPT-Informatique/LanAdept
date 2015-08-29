using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdept.Views.Place.ModelController
{
	public class ListeModel
	{
		public IEnumerable<PlaceSection> Sections { get; set; }

		public string Message { get; set; }

		public string Type { get; set; }
	}
}