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
		[EmailAddress(ErrorMessage = "L'adresse email doit être valide.")]
		[Display(Name = "Email")]
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
		[StringLength(100, ErrorMessage = "Votre mot de passe doit avoir au moins {2} caractères.", MinimumLength = 4)]
		[Display(Name = "Mot de passe")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirmer le mot de passe")]
		[Compare("Password", ErrorMessage ="Votre mot de passe et la confirmation de celui-ci sont différents.")]
		public string PasswordConfirmation { get; set; }

	}
}