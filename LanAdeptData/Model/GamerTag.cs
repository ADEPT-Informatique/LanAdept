using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class GamerTag
	{
		public int GamerTagID { get; set; }

		[Required]
		[StringLength(32)]
		public string Gamertag { get; set; }

		public virtual int UserID { get; set; }

		#region Navigation properties
		[ForeignKey("UserID")]
		public virtual User User { get; set; }

		public virtual ICollection<Team> Teams { get; set; }
		#endregion

        public override string ToString()
        {
            return Gamertag;
        }
	}
}
