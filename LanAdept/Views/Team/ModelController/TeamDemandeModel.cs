using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Teams.ModelController
{
	public class TeamDemandeModel
	{
		public List<Demande> Demandes { get; set; }

		public LanAdeptData.Model.Team Team { get; set; }
	}
}