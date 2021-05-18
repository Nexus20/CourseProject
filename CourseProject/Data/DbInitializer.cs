﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data {
    public static class DbInitializer {

        public static void Initialize2(CarContext context) {

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
                    Year = random.Next(1980, 2022)
                });
            }

            foreach (var car in cars) {
                context.Cars.Add(car);
            }

            context.SaveChanges();

        }

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

            var cars = new Car[] {
                new() {
                    BodyTypeId = 1, EngineVolume = 1.6, FuelTypeId = 1, Mileage = 0,
                    ModelId = 1, State = Car.CarState.New, TransmissionTypeId = 1, Price = 100000M, Year = 2006
                },
                new() {
                    BodyTypeId = 1, EngineVolume = 1.6, FuelTypeId = 2, Mileage = 0,
                    ModelId = 2, State = Car.CarState.New, TransmissionTypeId = 1, Price = 100000M, Year = 2007
                },
                new() {
                    BodyTypeId = 1, EngineVolume = 1.6, FuelTypeId = 1, Mileage = 0,
                    ModelId = 3, State = Car.CarState.New, TransmissionTypeId = 1, Price = 100000M, Year = 2008
                },
            };

            foreach (var car in cars) {
                context.Cars.Add(car);
            }

            context.SaveChanges();

        }

    }
}
