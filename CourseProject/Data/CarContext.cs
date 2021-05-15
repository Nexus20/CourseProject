using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data {
    public class CarContext : DbContext {

        public CarContext(DbContextOptions<CarContext> options) : base(options) {}

        public DbSet<Car> Cars { get; set; }

        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<TransmissionType> TransmissionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Car>().ToTable("Car");
            modelBuilder.Entity<CarModel>().ToTable("CarModel");
            //modelBuilder.Entity<CarModel>()
            //    .HasOne(cm => cm.Parent)
            //    .WithMany(cm => cm.Children)
            //    .HasForeignKey(cm => cm.ParentModelID)
            //    .IsRequired(false);


            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<FuelType>().ToTable("FuelType");
            modelBuilder.Entity<BodyType>().ToTable("BodyType");
            modelBuilder.Entity<TransmissionType>().ToTable("TransmissionType");
        }


    }
}
