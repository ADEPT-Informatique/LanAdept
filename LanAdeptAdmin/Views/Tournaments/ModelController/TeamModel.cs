using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdeptAdmin.Views.Tournaments.ModelController
{
	public class TeamModel
	{
		public int TeamID { get; set; }

		public string Name { get; set; }

		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public int UserID { get; set; }

		public User TeamLeader { get; set; }

		public Tournament Tournament { get; set; }

		public ICollection<User> Users { get; set; }

		public TeamModel()
		{

		}

		public TeamModel(Team team)
		{
			TeamID = team.TeamID;
			Name = team.Name;
			Tag = team.Tag;
			UserID = team.UserID;
			TeamLeader = team.TeamLeader;
			TournamentID = team.TournamentID;
			Tournament = team.Tournament;
			Users = team.Users;
		}
	}
}