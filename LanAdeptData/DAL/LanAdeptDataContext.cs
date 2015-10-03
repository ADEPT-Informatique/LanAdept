using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanAdeptData.Model;

namespace LanAdeptData.DAL
{
	public class LanAdeptDataContext : DbContext
	{
		public LanAdeptDataContext() : base("name=LanAdeptDataContext") 
		{ }

		public DbSet<User> Users { get; set; }
		public DbSet<Guest> Guests { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<LoginHistory> LoginHistories { get; set; }
		public DbSet<Place> Places { get; set; }
		public DbSet<PlaceSection> PlaceSections { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Team> Teams { get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<Gamer> Gamers { get; set; }
		public DbSet<Setting> Settings { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Demande> Demandes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}
	}
}
