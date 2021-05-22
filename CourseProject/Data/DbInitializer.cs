using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Data {
    public static class DbInitializer {

        public static async Task InitializeRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) {
            var adminEmail = "jack.gelder0804@gmail.com";
            var password = "ABCabc123_";

            if (await roleManager.FindByNameAsync("admin") == null) {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("manager") == null) {
                await roleManager.CreateAsync(new IdentityRole("manager"));
            }
            if (await roleManager.FindByNameAsync("user") == null) {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null) {
                var admin = new User() {
                    UserName = "admin",
                    Email = adminEmail
                };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

        public static void Initialize(CarContext context) {

            context.Database.EnsureCreated();

            if (context.Cars.Any()) {
                return;
            }

            var bodyTypes = new BodyType[] {
                new() {Name = "sedan"},
                new() {Name = "hatchback"},
                new() {Name = "station wagon "},
                new() {Name = "liftback"},
            };

            foreach (var bodyType in bodyTypes) {
                context.BodyTypes.Add(bodyType);
            }

            context.SaveChanges();

            var fuelTypes = new FuelType[] {
                new() {Name = "gas"},
                new() {Name = "diesel"},
            };

            foreach (var fuelType in fuelTypes) {
                context.FuelTypes.Add(fuelType);
            }

            context.SaveChanges();

            var transmissionTypes = new TransmissionType[] {
                new() {Name = "manual"},
                new() {Name = "automatic"},
            };

            foreach (var transmissionType in transmissionTypes) {
                context.TransmissionTypes.Add(transmissionType);
            }

            context.SaveChanges();

            var brands = new Brand[] {
                new() {Name = "Audi"},
                new() {Name = "Skoda"},
                new() {Name = "Volkswagen"},
            };

            foreach (var brand in brands) {
                context.Brands.Add(brand);
            }

            context.SaveChanges();

            var carModels = new CarModel[] {
                new() {BrandId = 3, Name = "Octavia"},
                new() {BrandId = 3, Name = "Fabia"},
                new() {BrandId = 2, Name = "Passat"},
                new() {BrandId = 2, Name = "Golf"},
                new() {BrandId = 1, Name = "A5"},
                new() {BrandId = 1, Name = "A7"},
            };

            foreach (var carModel in carModels) {
                context.CarModels.Add(carModel);
            }

            context.SaveChanges();

            List<Car> cars = new List<Car>();

            Random random = new Random();

            for (var i = 0; i < 45; i++) {
                cars.Add(new Car() {
                    BodyTypeId = 1,
                    EngineVolume = 1.6,
                    FuelTypeId = 1,
                    Mileage = 0,
                    ModelId = random.Next(1, 5),
                    State = Car.CarState.New,
                    TransmissionTypeId = 1,
                    Price = Convert.ToDecimal(random.Next(10000, 1000000)),
                    Count = random.Next(1, 4),
                    Color = "black"
                });
            }

            foreach (var car in cars) {
                context.Cars.Add(car);
            }

            context.SaveChanges();

            var dealers = new Dealer[] {
                new() {Name = "dealer audi", BrandId = 1, Email = "audi@audi.com", Phone = "+123456789000"},
                new() {Name = "dealer skoda", BrandId = 2, Email = "skoda@skoda.com", Phone = "+123456789001"},
                new() {
                    Name = "dealer volkswagen", BrandId = 3, Email = "volkswagen@volkswagen.com",
                    Phone = "+123456789002"
                },
            };

            foreach (var dealer in dealers) {
                context.Dealers.Add(dealer);
            }

            context.SaveChanges();

        }

    }
}
