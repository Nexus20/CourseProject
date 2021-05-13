using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class Car {

        public int ID { get; set; }

        public int Year { get; set; }
        public decimal Price { get; set; }

        public enum CarState {
            New,
            SecondHand
        }

        public CarState State { get; set; }

        public int ModelID { get; set; }
        public CarModel Model { get; set; }

        public double? EngineVolume { get; set; }
        public double Mileage { get; set; }

        public int FuelTypeID { get; set; }
        public FuelType FuelType { get; set; }

        public int BodyTypeID { get; set; }
        public BodyType BodyType { get; set; }

        public int TransmissionTypeID { get; set; }
        public TransmissionType TransmissionType { get; set; }

        public string ImagesDirectoryPath { get; set; }

    }
}
