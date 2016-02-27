using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Auth.ModelController
{
	public class ForgotPasswordModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}