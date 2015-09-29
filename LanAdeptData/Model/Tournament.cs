using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Tournament
	{
		public int TournamentID { get; set; }

		public DateTime? StartTime { get; set; }

		public DateTime CreationDate { get; set; }

		[ForeignKey("Game")]
		public virtual int GameID { get; set; }

		[ForeignKey("Organizer")]
		public virtual int? UserID { get; set; }

		#region Navigation properties
		public virtual Game Game { get; set; }

		public virtual User Organizer { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		#endregion
	}
}
