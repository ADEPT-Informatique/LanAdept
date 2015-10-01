using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Team
	{
		public int TeamID { get; set; }

		public string Name { get; set; }

		public string Tag { get; set; }

		public int TournamentID { get; set; }

		public virtual int UserID { get; set; }

		#region Navigation properties
		[ForeignKey("UserID")]
		public virtual User TeamLeader { get; set; }

		[ForeignKey("TournamentID")]
		public virtual Tournament Tournament { get; set; }

		public virtual ICollection<User> Users { get; set; }
		#endregion
	}
}
