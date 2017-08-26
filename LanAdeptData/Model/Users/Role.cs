using Microsoft.AspNet.Identity.EntityFramework;

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
