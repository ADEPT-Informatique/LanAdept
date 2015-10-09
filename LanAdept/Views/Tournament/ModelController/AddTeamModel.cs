using LanAdeptData.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LanAdept.Views.Tournament.ModelController
{
	public class AddTeamModel
	{
		[Required]
		[StringLength(30, MinimumLength = 4)]
		[DisplayName("Nom de l'équipe")]
		public string Name { get; set; }

		[StringLength(4, MinimumLength = 1)]
		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

        public int GamerTagId { get; set; }

        public IEnumerable<GamerTag> GamerTags { get; set; }

		public LanAdeptData.Model.Tournament Tournament { get; set; }
	}
}