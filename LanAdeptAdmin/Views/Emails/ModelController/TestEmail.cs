using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;

namespace LanAdeptAdmin.Emails
{
	public class TestEmail : Email
	{
		public string To { get; set; }
		public string Message { get; set; }
	}
}