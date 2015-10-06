using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Tournament.ModelController
{
	public class DetailsTeamModel
	{
		public string Name { get; set; }

		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

		public IEnumerable<GamerTag> GamerTags { get; set; }


		public DetailsTeamModel()
		{
		}

		public DetailsTeamModel(Team team)
		{
			Name = team.Name;
			Tag = team.Tag;
			TournamentID = team.TournamentID;
			TeamLeaderTag = team.TeamLeaderTag;
			GamerTags = team.GamerTags;
		}
	}
}