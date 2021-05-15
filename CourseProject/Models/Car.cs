using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class Car {

        public int Id { get; set; }

        public int Year { get; set; }
        public decimal Price { get; set; }

        public enum CarState {
            New,
            SecondHand
        }

        public CarState State { get; set; }

        public int ModelId { get; set; }
        public CarModel Model { get; set; }

        public double? EngineVolume { get; set; }
        public double Mileage { get; set; }

        public int FuelTypeId { get; set; }
        public FuelType FuelType { get; set; }

        public int BodyTypeId { get; set; }
        public BodyType BodyType { get; set; }

        public int TransmissionTypeId { get; set; }
        public TransmissionType TransmissionType { get; set; }

    }
}
