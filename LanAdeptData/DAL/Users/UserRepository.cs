using System.Collections.Generic;
using System.Linq;
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

		public IEnumerable<User> GetUsersByName(string name)
		{
			return Get().Where(n => n.CompleteName.Contains(name));
		}

		public IEnumerable<User> SearchUsersByNameAndEmail(string query)
		{
			return Get(u => u.CompleteName.Contains(query) || u.Email.Contains(query));
		}

		public User GetUserByBarCode(string barcode)
		{
			return Get().Where(model => model.Barcode == barcode).FirstOrDefault();
		}

	}
}
