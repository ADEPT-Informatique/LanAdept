using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.Model
{
	public class Role : IdentityRole
	{
		public Role() : base() { }
		public Role(string name) : base(name) { }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool HideRole { get; set; }
	}
}
