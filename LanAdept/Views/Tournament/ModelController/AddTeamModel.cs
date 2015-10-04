using LanAdeptData.Model;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LanAdept.Views.Tournament.ModelController
{
	public class AddTeamModel
	{
		public string Name { get; set; }

		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

        public int GamerTagId { get; set; }

        public IEnumerable<GamerTag> GamerTags { get; set; }
        //public IEnumerable<SelectListItem> LeaderTags { get; set; }

		public LanAdeptData.Model.Tournament Tournament { get; set; }
	}
}