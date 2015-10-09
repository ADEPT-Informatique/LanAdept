using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournament.ModelController
{
	public class TeamModel
	{
		public bool IsMyTeam { get; set; }

		public bool IsMyTeamForTeamLeader { get; set; }

		public bool IsTeamDemande { get; set; }

		public string Name { get; set; }

		public string Tag { get; set; }

		public IEnumerable<GamerTag> Gamertags { get; set; }
	}
}