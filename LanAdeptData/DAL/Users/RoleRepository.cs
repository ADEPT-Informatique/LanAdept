using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Users
{
    public class RoleRepository : GenericRepository<Role>
	{
		public RoleRepository(LanAdeptDataContext context) : base(context) { }
	}
}
