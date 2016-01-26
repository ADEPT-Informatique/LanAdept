namespace LanAdeptData.Migrations
{
	using DAL;
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using Model;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<LanAdeptDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(LanAdeptDataContext context)
        {
			SeedRoles(context);
			SeedUsers(context);
		}

		private void SeedRoles(LanAdeptDataContext context)
		{
			if (!context.Roles.Any(r => r.Name == "admin"))
			{
				var store = new RoleStore<IdentityRole>(context);
				var manager = new RoleManager<IdentityRole>(store);
				var role = new IdentityRole { Name = "admin" };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "owner"))
			{
				var store = new RoleStore<IdentityRole>(context);
				var manager = new RoleManager<IdentityRole>(store);
				var role = new IdentityRole { Name = "owner" };

				manager.Create(role);
			}
		}

		private void SeedUsers(LanAdeptDataContext context)
		{
			var store = new UserStore<User>(context);
			var manager = new UserManager<User>(store);

			if (!context.Users.Any(u => u.Email == "admin@cgagnier.ca"))
			{
				var user = new User { UserName = "admin@cgagnier.ca", Email = "admin@cgagnier.ca", CompleteName = "Administrateur" };

				manager.Create(user, "fafa1234");
				manager.AddToRole(user.Id, "admin");
				manager.AddToRole(user.Id, "owner");
			}
		}
	}
}
