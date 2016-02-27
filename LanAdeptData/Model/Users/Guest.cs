using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model.Users
{
	public class Guest
	{
		public int GuestID { get; set; }

		public string CompleteName { get; set; }
	}
}
