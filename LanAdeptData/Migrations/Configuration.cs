namespace LanAdeptData.Migrations
{
	using DAL;
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using Model;
	using Model.Settings;
	using Model.Users;
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
            SeedPlaces(context);


            context.SaveChanges();
		}

		private void SeedRoles(LanAdeptDataContext context)
		{
			if (!context.Roles.Any(r => r.Name == "owner"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "owner", Description = "Permet de contourner toutes les vérifications d'autorisation.", HideRole = true };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "admin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "admin", DisplayName = "Administrateur", Description = "Permet à l'utilisateur de se connecter sur le panneau d'administration (Nécéssaire pour tous les autres rôles)." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "generalAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "generalAdmin", DisplayName = "Admin. général", Description = "Permet de configurer les paramètres généraux du LAN, tel que la description ou la date de début du LAN." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "userAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "userAdmin", DisplayName = "Admin. utilisateurs", Description = "Permet de voir la liste des utilisateurs et leurs détails, ainsi que de modifier les utilisateurs et leurs permissions." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "placeAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "placeAdmin", DisplayName = "Admin. places", Description = "Permet de gérer les réservations de places du LAN." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "tournamentAdmin"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "tournamentAdmin", DisplayName = "Admin. tournois", Description = "Permet de créer, gérer et supprimer tous les tournois." };

				manager.Create(role);
			}
			if (!context.Roles.Any(r => r.Name == "tournamentMod"))
			{
				var store = new RoleStore<Role>(context);
				var manager = new RoleManager<Role>(store);
				var role = new Role { Name = "tournamentMod", DisplayName = "Org. tournois", Description = "Permet d'organiser les tournois dans lesquels il est autorisé." };

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
					Description = "Description du LAN à écrire",
					Rules = "Règlements à écrire",
					StartDate = DateTime.Now.AddMonths(1),
					EndDate = DateTime.Now.AddMonths(2),
					PlaceReservationStartDate = DateTime.Now.AddMonths(1).AddDays(-14),
					TournamentSubsciptionStartDate = DateTime.Now.AddMonths(1).AddDays(-7),
					NbDaysBeforeRemember = 5,
					RememberEmailContent = "Non utilisé pour le moment",
					SendRememberEmail = false,
                    PublicKeyId= "3788b701-1b85-4864-bc58-49ff85ca2c14",
                    EventKeyId= "cd826557-5c7e-48d2-96e2-2523128a6e25",
                    SecretKeyId = "7cb1e091-2f53-44fa-b3fd-24159a31ba5b"
                };

				context.Settings.Add(settings);
			}
		}
        private void SeedPlaces(LanAdeptDataContext context)
        {
            for(char i = 'A'; i <= 'I'; i++)
            {
                for (int y = 1; y < 25; y++)
                {
                    Model.Places.Place place = new Model.Places.Place();
                    place.SeatsId = i + "-" + y.ToString();
                    context.Places.Add(place);
                }
            }
        }
	}
}
