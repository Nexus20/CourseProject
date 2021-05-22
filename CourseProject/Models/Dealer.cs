using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class Dealer {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "String length must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+\d{10, 14}$", ErrorMessage = "Enter the phone number in the format  \"+123456789000\"")]
        public string Phone { get; set; }

        [Required]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

    }
}
