using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Tournaments
{
	public class Team
	{
		public int TeamID { get; set; }

		[Required]
		[StringLength(64)]
		public string Name { get; set; }

		[StringLength(4)]
		public string Tag { get; set; }

		public bool IsComplete { get; set; }

		public bool IsReady { get; set; }

		public bool IsConfirmed { get; set; }

		public int TournamentID { get; set; }

		#region Navigation properties

        public virtual GamerTag TeamLeaderTag { get; set; }

		[ForeignKey("TournamentID")]
		public virtual Tournament Tournament { get; set; }

		public virtual ICollection<GamerTag> GamerTags { get; set; }

		public virtual ICollection<Demande> Demandes { get; set; }

		#endregion
	}
}
