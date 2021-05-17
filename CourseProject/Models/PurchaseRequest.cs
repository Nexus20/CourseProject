using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class PurchaseRequest {

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

    }
}
