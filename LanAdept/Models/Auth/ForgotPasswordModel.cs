using System.ComponentModel.DataAnnotations;

namespace LanAdept.Models
{
    public class ForgotPasswordModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}