using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanAdept.Views.Auth.ModelController
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[EmailAddress(ErrorMessage = "L'adresse email doit être valide.")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(60, MinimumLength = 4)]
		[Display(Name = "Nom complet")]
		public string CompleteName { get; set; }

	}
}