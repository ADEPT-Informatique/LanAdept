using LanAdeptData.Model;
using LanAdeptData.Model.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Teams.ModelController
{
	public class TeamDemandeModel
	{
		public List<Demande> Demandes { get; set; }

		public Team Team { get; set; }
	}
}