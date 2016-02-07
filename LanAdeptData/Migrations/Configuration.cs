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
			SeedGlobalSettings(context);

			context.SaveChanges();
		}

		private void SeedRoles(LanAdeptDataContext context)
		{
			if (!context.Roles.Any(r => r.Name == "owner"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "owner", Description = "Permet de contourner toutes les v�rifications d'autorisation.", HideRole = true };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "admin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "admin", DisplayName = "Administrateur", Description = "Permet � l'utilisateur de se connecter sur le panneau d'administration (N�c�ssaire pour tous les autres r�les)." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "generalAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "generalAdmin", DisplayName = "Admin. g�n�ral", Description = "Permet de configurer les param�tres g�n�raux du LAN, tel que la description ou la date de d�but du LAN." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "userAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "userAdmin", DisplayName = "Admin. utilisateurs", Description = "Permet de voir la liste des utilisateurs et leurs d�tails, ainsi que de modifier les utilisateurs et leurs permissions." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "placeAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "placeAdmin", DisplayName = "Admin. places", Description = "Permet de g�rer les r�servations de places du LAN." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "tournamentAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "tournamentAdmin", DisplayName = "Admin. tournois", Description = "Permet de cr�er, g�rer et supprimer tous les tournois." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "tournamentMod"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "tournamentMod", DisplayName = "Mod. tournois", Description = "Permet de g�rer les tournois dans lesquels il est autoris�." };

				manager.Create(role);
			}
		}

		private void SeedUsers(LanAdeptDataContext context)
		{
			var store = new UserStore<User>(context);
			var manager = new UserManager<User>(store);

			if (!context.Users.Any(u => u.Email == "admin@lanadept.com"))
			{
				var user = new User { UserName = "admin@lanadept.com", Email = "admin@lanadept.com", CompleteName = "Administrateur", EmailConfirmed = true };

				manager.Create(user, "fafa1234");
				manager.AddToRole(user.Id, "owner");
				manager.AddToRole(user.Id, "admin");
			}
		}

		private void SeedGlobalSettings(LanAdeptDataContext context)
		{
			if(!context.Settings.Any())
			{
				var settings = new Setting()
				{
					Description = "Description du LAN � �crire",
					Rules = "R�glements � �crire",
					StartDate = DateTime.Now.AddMonths(1),
					EndDate = DateTime.Now.AddMonths(2),
					NbDaysBeforeRemember = 5,
					RememberEmailContent = "Non utilis� pour le moment",
					SendRememberEmail = false
				};

				context.Settings.Add(settings);
			}
		}
	}
}
