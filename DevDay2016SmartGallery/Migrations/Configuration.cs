namespace DevDay2016SmartGallery.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DevDay2016SmartGallery.DAL.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DevDay2016SmartGallery.DAL.AppDbContext context)
        {
            
        }
    }
}
