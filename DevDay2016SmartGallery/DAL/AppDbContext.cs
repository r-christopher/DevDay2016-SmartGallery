using DevDay2016SmartGallery.Models;
using Microsoft.ProjectOxford.Face.Contract;
using System.Data.Entity;


namespace DevDay2016SmartGallery.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Models.Face>  Faces { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Models.Person> Persons { get; set; }


        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ComplexType<FaceRectangle>();
            modelBuilder.ComplexType<FaceAttributes>();

            modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.HeadPose);
            modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.FacialHair);

            base.OnModelCreating(modelBuilder);
        }
    }
}