using LanAdeptData.Model;
using Postal;

namespace LanAdept.Models
{
    public class ResetPasswordEmail : Email
	{
		public User User { get; set; }
		public string ResetLink { get; set; }
	}
}