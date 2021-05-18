using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class FeaturedCar {

        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
