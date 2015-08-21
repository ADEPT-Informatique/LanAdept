using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanAdeptData.Model;

namespace LanAdept.Views.Place.ViewModel
{
	public class ListeModel
	{
		public IEnumerable<PlaceSection> Sections { get; set; }
	}
}