using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject.Models
{
    public class Car
    {

        public enum CarState
        {
            New,
            SecondHand
        }

        public enum CarPresence
        {
            InStock,
            Sold,
            AwaitingDelivery,
            BookedOrSold,
        }

        public Car()
        {
            Presence = Count == 0 ? CarPresence.BookedOrSold : CarPresence.InStock;
        }

        public int Id { get; set; }

        public int? Year { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum value for this field is 0")]
        public int Count { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [Required]
        public CarState State { get; set; }

        public CarPresence Presence { get; set; }

        [Required]
        [Display(Name = "Model")]
        public int ModelId { get; set; }
        public CarModel Model { get; set; }

        [Display(Name = "Engine volume")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Minimum value for this field is 0.1")]
        public double? EngineVolume { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum value for this field is 0")]
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

    }
}
