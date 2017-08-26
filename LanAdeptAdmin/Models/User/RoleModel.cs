using LanAdeptData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanAdeptAdmin.Models
{
	public class RoleModel
	{
		public string RoleId { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool UserInRole { get; set; }

		public RoleModel() {}

		public RoleModel(Role role)
		{
			RoleId = role.Id;
			DisplayName = role.DisplayName;
			Description = role.Description;
		}
	}
}