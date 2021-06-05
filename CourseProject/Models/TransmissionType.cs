using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Models {
    public class TransmissionType {

        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "String length must be between 3 and 30 characters")]
        [Remote(action: "CheckTransmissionType", controller: "Cars", areaName: "Admin", ErrorMessage = "This transmission type already exists")]
        [Display(Name = "Type")]
        public string Name { get; set; }
    }
}
