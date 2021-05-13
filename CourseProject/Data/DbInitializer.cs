using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CourseProject.Models;

namespace CourseProject.Data {
    public static class DbInitializer {

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
                new() {Name = "Skoda"},
                new() {Name = "Volkswagen"},
                new() {Name = "Audi"},
            };

            foreach (var brand in brands) {
                context.Brands.Add(brand);
            }

            context.SaveChanges();

            var carModels = new CarModel[] {
                new() {BrandID = 1, Name = "Octavia", ParentModelID = 0},
                new() {BrandID = 1, Name = "Fabia", ParentModelID = 0},
                new() {BrandID = 2, Name = "Passat", ParentModelID = 0},
                new() {BrandID = 2, Name = "Golf", ParentModelID = 0},
                new() {BrandID = 3, Name = "A5", ParentModelID = 0},
                new() {BrandID = 3, Name = "A7", ParentModelID = 0},
            };

            foreach (var carModel in carModels) {
                context.CarModels.Add(carModel);
            }

            context.SaveChanges();

            var cars = new Car[] {
                new() {
                    BodyTypeID = 1, EngineVolume = 1.6, FuelTypeID = 1, ImagesDirectoryPath = "", Mileage = 0,
                    ModelID = 1, State = Car.CarState.New, TransmissionTypeID = 1, Price = 100000M, Year = 2006
                },
                new() {
                    BodyTypeID = 1, EngineVolume = 1.6, FuelTypeID = 2, ImagesDirectoryPath = "", Mileage = 0,
                    ModelID = 2, State = Car.CarState.New, TransmissionTypeID = 1, Price = 100000M, Year = 2007
                },
                new() {
                    BodyTypeID = 1, EngineVolume = 1.6, FuelTypeID = 1, ImagesDirectoryPath = "", Mileage = 0,
                    ModelID = 3, State = Car.CarState.New, TransmissionTypeID = 1, Price = 100000M, Year = 2008
                },
            };

            foreach (var car in cars) {
                context.Cars.Add(car);
            }

            context.SaveChanges();

        }

    }
}
