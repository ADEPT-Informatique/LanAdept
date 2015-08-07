using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Users
{
	public class PermissionRepository : GenericRepository<Permission>
	{
		public PermissionRepository(LanAdeptDataContext context) : base(context) { }

		public Permission GetPermissionByName(string permissionName)
		{
			return Get(perm => perm.Name == permissionName).FirstOrDefault();
		}
	}
}
