using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Users
{
	public class LoginHistoryRepository : GenericRepository<LoginHistory>
	{
		public LoginHistoryRepository(LanAdeptDataContext context)
			: base(context)
		{ }

		/// <summary>
		/// Create a new login history for a user
		/// </summary>
		/// <param name="user">User that just logged in</param>
		/// <returns>the LoginHistory that was created</returns>
		public LoginHistory CreateHistoryFor(User user)
		{
			LoginHistory hist = new LoginHistory();
			hist.Date = DateTime.Now;
			hist.IpAddress = HttpContext.Current.Request.UserHostAddress;
			hist.UserID = user.UserID;

			return hist;
		}
	}
}
