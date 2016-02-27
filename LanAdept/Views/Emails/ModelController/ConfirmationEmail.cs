using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanAdeptData.Model;
using Postal;
using LanAdeptData.Model.Users;

namespace LanAdept.Emails
{
	public class ConfirmationEmail : Email
	{
		public User User { get; set; }
		public string ConfirmationToken { get; set; }
	}
}