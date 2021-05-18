using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models {
    public class User : IdentityUser {

        public ICollection<FeaturedCar> FeaturedCars;

    }
}
