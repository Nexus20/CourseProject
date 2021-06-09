using System.ComponentModel.DataAnnotations;

namespace CourseProject.Models
{
    public class SupplyRequest
    {
        public enum SupplyRequestState
        {
            New,
            Sent,
            Closed
        }

        public SupplyRequest()
        {
            State = SupplyRequestState.New;
        }

        [Display(Name = "Request ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Dealer")]
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum value for this field is 1")]
        [Display(Name = "Cars count")]
        public int Count { get; set; }

        public SupplyRequestState State { get; set; }
    }
}
