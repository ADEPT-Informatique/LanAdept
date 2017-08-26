using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
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