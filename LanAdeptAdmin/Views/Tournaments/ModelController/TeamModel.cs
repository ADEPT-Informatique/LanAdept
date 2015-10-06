using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdeptAdmin.Views.Tournaments.ModelController
{
	public class TeamModel
	{
		public int TeamID { get; set; }

		public string Name { get; set; }

		public string Tag { get; set; }

		public bool IsComplete { get; set; }

		public bool IsReady { get; set; }

		public bool IsConfirmed { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

		public Tournament Tournament { get; set; }

        public ICollection<GamerTag> TeamGamerTags { get; set; }

		public TeamModel()
		{

		}

		public TeamModel(Team team)
		{
			TeamID = team.TeamID;
			Name = team.Name;
			Tag = team.Tag;
            TeamLeaderTag = team.TeamLeaderTag;
			TournamentID = team.TournamentID;
			Tournament = team.Tournament;
			TeamGamerTags = team.GamerTags;
			IsComplete = team.IsComplete;
			IsConfirmed = team.IsConfirmed;
			IsReady = team.IsReady;
		}
	}
}