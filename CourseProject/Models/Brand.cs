using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Models {
    public class Brand {

        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "String length must be between 3 and 30 characters")]
        [Remote(action: "CheckBrand", controller: "CarModels", areaName: "Admin", ErrorMessage = "This brand already exists")]
        public string Name { get; set; }

        public ICollection<Dealer> Dealers { get; set; }

    }
}
