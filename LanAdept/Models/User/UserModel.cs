using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
{
    public class UserModel
	{
		[Required]
		[Display(Name = "Nom complet")]
		public string CompleteName { get; set; }
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}