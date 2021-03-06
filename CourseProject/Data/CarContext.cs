using CourseProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data {
    public class CarContext : IdentityDbContext<User> {

        public CarContext(DbContextOptions<CarContext> options) : base(options) {}

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<TransmissionType> TransmissionTypes { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<FeaturedCar> FeaturedCars { get; set; }

        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<SupplyRequest> SupplyRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Car>().ToTable("Car");
            modelBuilder.Entity<CarModel>().ToTable("CarModel");
            modelBuilder.Entity<CarImage>().ToTable("CarImages");

            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<FuelType>().ToTable("FuelType");
            modelBuilder.Entity<BodyType>().ToTable("BodyType");
            modelBuilder.Entity<TransmissionType>().ToTable("TransmissionType");
            modelBuilder.Entity<PurchaseRequest>().ToTable("PurchaseRequest");
            modelBuilder.Entity<FeaturedCar>().ToTable("FeaturedCar");

            modelBuilder.Entity<Dealer>().ToTable("Dealers");
            modelBuilder.Entity<SupplyRequest>().ToTable("SupplyRequests");

            base.OnModelCreating(modelBuilder);
        }


    }
}
