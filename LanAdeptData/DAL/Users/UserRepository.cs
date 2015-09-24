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

        public User GetUserByName(string name)
        {
            return Get().Where(n => n.CompleteName.Contains(name)).FirstOrDefault();
        }

        public User GetUserByBarCode(string codeBare)
        {
            return Get().Where(model => model.CodeBare == codeBare).FirstOrDefault();
        }
	}
}
