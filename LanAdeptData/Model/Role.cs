using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Role
	{
		public int RoleID { get; set; }

		public string Name { get; set; }

		public int PermissionLevel { get; set; }

		public bool IsReadOnly { get; set; }

		public bool IsDefaultRole { get; set; }

		public bool IsOwnerRole { get; set; }

		public bool IsUnconfirmedRole { get; set; }
	}
}
