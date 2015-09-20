using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LanAdeptData.Validation;

namespace LanAdept.Views.Auth.ModelController
{
	public class RegisterModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[UniqueEmail]
		public string Email { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Confirmer l'adresse email")]
		[Compare("Email")]
		public string EmailConfirmation { get; set; }

		[Required]
		[StringLength(60, MinimumLength = 4)]
		[Display(Name = "Nom complet")]
		public string CompleteName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Mot de passe")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirmer le mot de passe")]
		[Compare("Password")]
		public string PasswordConfirmation { get; set; }

	}
}