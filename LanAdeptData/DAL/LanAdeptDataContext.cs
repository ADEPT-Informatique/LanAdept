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
		public LanAdeptDataContext() : base("name=LanAdeptDataContext") { }

		public DbSet<Test> Tests { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}
	}
}
