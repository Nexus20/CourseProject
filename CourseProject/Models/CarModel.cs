using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Models
{
    public class CarModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Model")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "String length must be between 2 and 30 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Brand")]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        [Display(Name = "Parent Model")]
        public int? ParentId { get; set; }
        public virtual CarModel Parent { get; set; }

        public virtual ICollection<CarModel> Children { get; set; }

    }
}
