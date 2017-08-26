using LanAdeptData.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
{
    public class AddTeamModel
	{
		[Required]
		[StringLength(30, MinimumLength = 4, ErrorMessage="Le nom de l'équipe doit être entre 4 et 30 charactères")]
		[DisplayName("Nom de l'équipe")]
		public string Name { get; set; }

		[StringLength(4, MinimumLength = 1, ErrorMessage="Le tag doit comporter entre 1 et 4 charactères")]
		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public GamerTag TeamLeaderTag { get; set; }

        public int GamerTagId { get; set; }

        public IEnumerable<GamerTag> GamerTags { get; set; }

		public Tournament Tournament { get; set; }
	}
}