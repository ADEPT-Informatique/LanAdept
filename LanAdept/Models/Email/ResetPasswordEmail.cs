using LanAdeptData.Model;

namespace LanAdept.Models
{
    public class ResetPasswordEmail //: Email // TODO fix email
	{
		public User User { get; set; }
		public string ResetLink { get; set; }
	}
}