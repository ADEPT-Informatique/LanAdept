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
		{
			Database.SetInitializer<LanAdeptDataContext>(new DataInitializer());
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<LoginHistory> LoginHistories { get; set; }
		public DbSet<Place> Places { get; set; }
		public DbSet<PlaceSection> PlaceSections { get; set; }
		public DbSet<PlaceHistory> PlaceHistories { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}
	}
}
