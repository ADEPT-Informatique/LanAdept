using LanAdeptData.Model.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Tournaments
{
	public class Tournament
	{
		public int TournamentID { get; set; }

		public DateTime? StartTime { get; set; }

		public DateTime CreationDate { get; set; }

		public bool IsPublished { get; set; }

		public bool IsStarted { get; set; }

		public bool IsOver { get; set; }

		public string Info { get; set; }

		public int MaxPlayerPerTeam { get; set; }

		public string Game { get; set; }

        public string ChallongeUrl { get; set; }

        #region Navigation properties

        public virtual string UserID { get; set; }

        [ForeignKey("UserID")]
		public virtual User Organizer { get; set; }

		public virtual ICollection<Team> Teams { get; set; }

		#endregion
	}
}