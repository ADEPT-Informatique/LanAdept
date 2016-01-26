﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Auth.ModelController
{
	public class LoginModel
	{
		[Required]
		[EmailAddress(ErrorMessage = "L'adresse email doit être valide.")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Mot de passe")]
		public string Password { get; set; }

		[Display(Name = "Garder ma session active")]
		public bool RememberMe { get; set; }

	}
}