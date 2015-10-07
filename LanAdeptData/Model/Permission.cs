using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Permission
	{
		public int PermissionID { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }

		public string Description { get; set; }

		public int MinimumRoleLevel { get; set; }

		public bool IsReadOnly { get; set; }

	}
}
