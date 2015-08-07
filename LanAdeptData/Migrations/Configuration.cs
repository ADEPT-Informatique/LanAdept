namespace LanAdeptData.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LanAdeptData.DAL.LanAdeptDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LanAdeptData.DAL.LanAdeptDataContext context)
        { }
    }
}
