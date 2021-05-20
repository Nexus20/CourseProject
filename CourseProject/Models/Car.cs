using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class Car {

        public int Id { get; set; }

        public int? Year { get; set; }

        public string Color { get; set; }

        public int Count { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public enum CarState {
            New,
            SecondHand
        }

        public CarState State { get; set; }

        public enum CarPresence {
            InStock,
            Sold,
            AwaitingDelivery,
            // TODO maybe we need one more state called "BookedOrSold"
            // TODO add changing state to "AwaitingDelivery" only after creating and sending supply request
        }

        public CarPresence Presence { get; set; }

        public int ModelId { get; set; }
        public CarModel Model { get; set; }

        [Display(Name = "Engine Volume")]
        public double? EngineVolume { get; set; }
        public double Mileage { get; set; }

        public int FuelTypeId { get; set; }
        [Display(Name = "Fuel")]
        public FuelType FuelType { get; set; }

        public int BodyTypeId { get; set; }
        [Display(Name = "Body")]
        public BodyType BodyType { get; set; }

        public int? TransmissionTypeId { get; set; }
        [Display(Name = "Transmission")]
        public TransmissionType TransmissionType { get; set; }

        public ICollection<CarImage> CarImages { get; set; }

        public Car() {
            Presence = CarPresence.InStock;
        }

    }
}
