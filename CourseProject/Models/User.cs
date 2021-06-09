using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "Login")]
        public override string UserName { get; set; }
        public ICollection<FeaturedCar> FeaturedCars;
        public ICollection<PurchaseRequest> PurchaseRequests;
    }
}
