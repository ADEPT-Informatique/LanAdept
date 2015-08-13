using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.Model;

namespace LanAdeptCore.Service.ServiceResult
{
	public class TryLoginResult
	{
		public bool HasSucceeded { get; set; }

		public User User { get; set; }

		public string Reason { get; set; }
	}
}
