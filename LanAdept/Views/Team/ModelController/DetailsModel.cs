using LanAdeptData.Model.Tournaments;
using System.Collections.Generic;

namespace LanAdept.Views.Teams.ModelController
{
	public class DetailsModel
	{
		public string Name { get; set; }

		public string Tag { get; set; }

		public int MaxPlayerPerTeam { get; set; }

		public int TeamID { get; set; }

		public int TournamentID { get; set; }

		public Tournament Tournament { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

		public IEnumerable<GamerTag> GamerTags { get; set; }

		public IEnumerable<Demande> Demandes { get; set; }
	}
}