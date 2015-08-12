using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptData.DAL
{
	public class DataInitializer : CreateDatabaseIfNotExists<LanAdeptDataContext>
	{
		protected override void Seed(LanAdeptDataContext context)
		{
			//Le ID des users commence à 2973 (chiffre random) pour que le user soit pas genre "Check je suis le premier!!1"
			context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 2973)");
		}
	}
}
