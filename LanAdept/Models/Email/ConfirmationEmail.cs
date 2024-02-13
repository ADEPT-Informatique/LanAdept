using LanAdeptData.Model;

namespace LanAdept.Models
{
    public class ConfirmationEmail //: Email // TODO FIX EMAIL
	{
		public User User { get; set; }
		public string ConfirmationToken { get; set; }
	}
}