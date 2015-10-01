using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdept.Views.Tournament.ModelController
{
	public class TeamModel
	{
		public string Name { get; set; }

		public int TournamentID { get; set; }

		public int UserID { get; set; }

		public User TeamLeader { get; set; }

		public LanAdeptData.Model.Tournament Tournament { get; set; }

		public TeamModel()
		{

		}

		public TeamModel(Team team)
		{
			Name = team.Name;
			UserID = team.UserID;
			TeamLeader = team.TeamLeader;
		}
	}
}