using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.DAL.Generic;
using LanAdeptData.Model;

namespace LanAdeptData.DAL.Users
{
	public class RoleRepository : GenericRepository<Role>
	{
		public RoleRepository(LanAdeptDataContext context) : base(context) { }

		/// <summary>
		/// Get the default user role
		/// </summary>
		public Role GetDefaultRole()
		{
			return Get(role => role.IsDefaultRole).First();
		}

		/// <summary>
		/// Get the temporary role assigned to user who have not yet confirmed their email
		/// </summary>
		public Role GetUnconfirmedRole()
		{
			return Get(role => role.IsUnconfirmedRole).First();
		}

		/// <summary>
		/// Get the highest rank, which will also have every rights possible
		/// </summary>
		public Role GetOwnerRole()
		{
			return Get(role => role.IsOwnerRole).First();
		}

	}
}
