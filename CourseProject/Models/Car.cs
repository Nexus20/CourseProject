using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class Car {
        public int Id { get; set; }

        [StringLength(4, ErrorMessage = "Enter correct year")]
        public int? Year { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public enum CarState {
            New,
            SecondHand
        }

        [Required]
        public CarState State { get; set; }

        public enum CarPresence {
            InStock,
            Sold,
            AwaitingDelivery,
            BookedOrSold,
        }

        public CarPresence Presence { get; set; }

        [Required]
        public int ModelId { get; set; }
        public CarModel Model { get; set; }

        [Display(Name = "Engine Volume")]
        public double? EngineVolume { get; set; }
        public double? Mileage { get; set; }

        [Required]
        public int FuelTypeId { get; set; }
        [Display(Name = "Fuel")]
        public FuelType FuelType { get; set; }

        [Required]
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
