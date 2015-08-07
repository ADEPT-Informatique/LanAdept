using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Users
{
	public class UserRepository : GenericRepository<User>
	{
		public UserRepository(LanAdeptDataContext context) : base(context) { }

		public User GetUserByEmail(string email)
		{
			return Get(u => u.Email == email).FirstOrDefault();
		}

		public User GetUserByUsername(string username)
		{
			return Get(u => u.Username == username).FirstOrDefault();
		}
	}
}
