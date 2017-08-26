using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanAdeptData.Model
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
