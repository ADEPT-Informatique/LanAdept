using LanAdeptData.Model;
using Postal;

namespace LanAdept.Models
{
    public class ConfirmationEmail : Email
	{
		public User User { get; set; }
		public string ConfirmationToken { get; set; }
	}
}