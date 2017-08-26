using LanAdeptData.Model;
using System.Collections.Generic;

namespace LanAdept.Models
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