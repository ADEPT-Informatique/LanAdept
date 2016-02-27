using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Users
{
	public class LoginHistory
	{
		public int LoginHistoryID { get; set; }

		[ForeignKey("User")]
		public int UserID { get; set; }

		public virtual User User { get; set; }

		public DateTime Date { get; set; }

		public string IpAddress { get; set; }

	}
}
