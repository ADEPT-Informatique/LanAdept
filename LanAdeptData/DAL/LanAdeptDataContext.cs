using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using LanAdeptData.Model.Users;
using LanAdeptData.Model.Places;
using LanAdeptData.Model.Tournaments;
using LanAdeptData.Model.Settings;
using LanAdeptData.Model.Maps;
using LanAdeptData.Model.Canteen;

namespace LanAdeptData.DAL
{
	public class LanAdeptDataContext : IdentityDbContext<User>
	{
		public static LanAdeptDataContext Create()
		{
			return new LanAdeptDataContext();
		}

		public LanAdeptDataContext() : base("name=LanAdeptDataContext")
		{ }

		public DbSet<Guest> Guests { get; set; }
		public DbSet<Place> Places { get; set; }
		public DbSet<PlaceSection> PlaceSections { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<Tournament> Tournaments { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<GamerTag> Gamertags { get; set; }
		public DbSet<Setting> Settings { get; set; }
		public DbSet<Map> Maps { get; set; }
		public DbSet<Demande> Demandes { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Item> Items { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			modelBuilder.Entity<GamerTag>()
				.HasMany<Team>(g => g.Teams)
				.WithMany(t => t.GamerTags)
				.Map(tg =>
						{
							tg.MapLeftKey("GamerTagID");
							tg.MapRightKey("TeamID");
							tg.ToTable("GamertagTeam");
						});

			base.OnModelCreating(modelBuilder);
		}
	}
}
