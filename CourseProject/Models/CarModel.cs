using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Models {
    public class CarModel {

        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "String length must be between 3 and 30 characters")]
        [Remote(action: "CheckModel", controller: "CarModels", areaName: "Admin", ErrorMessage = "This brand already exists")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int? ParentId { get; set; }

        [Display(Name = "Parent Model")]
        public virtual CarModel Parent { get; set; }
        public virtual ICollection<CarModel> Children { get; set; }

    }
}
