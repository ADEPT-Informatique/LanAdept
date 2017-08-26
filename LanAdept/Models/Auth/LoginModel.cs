using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
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

		[Display(Name = "Rester connecté")]
		public bool RememberMe { get; set; }

	}
}