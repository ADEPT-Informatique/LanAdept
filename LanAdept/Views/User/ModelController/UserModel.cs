using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.User.ModelController
{
	public class UserModel
	{
		public string UserId { get; set; }
		[Required]
		public string CompleteName { get; set; }
		[Required]
		public string Email { get; set; }
	}
}