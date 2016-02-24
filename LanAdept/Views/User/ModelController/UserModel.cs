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
		[Display(Name = "Nom complet")]
		public string CompleteName { get; set; }
		[Required]
		[Display(Name = "Email")]
		[LanAdeptData.Validation.UniqueEmail]
		public string Email { get; set; }
	}
}