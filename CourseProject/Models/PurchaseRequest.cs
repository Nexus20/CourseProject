using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class PurchaseRequest {

        public enum RequestState {
            New,
            Processing,
            Closed,
            Canceled
        }

        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Email { get; set; }

        public RequestState State { get; set; }

        public string FullName => $"{Firstname} {Surname}";

        public string ClientId { get; set; }
        [ForeignKey("ClientId")]
        public User Client { get; set; }

        public string ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public User Manager { get; set; }

        [DisplayFormat(DataFormatString = "{0:f}")]
        [Display(Name = "Application date")]
        public DateTime ApplicationDate { get; set; }

        [Display(Name = "Car available")]
        public bool CarAvailability { get; set; }

        public PurchaseRequest() {
            State = RequestState.New;
        }

    }
}
