using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Auth.ModelController
{
	public class LoginModel
	{
		[Required]
		[EmailAddress(ErrorMessage = "L'adresse Email doit être valide")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Mot de passe")]
		public string Password { get; set; }

		public string ReturnURL { get; set; }
	}
}