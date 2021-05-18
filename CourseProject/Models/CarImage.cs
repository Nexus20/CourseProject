using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class CarImage {

        public int Id { get; set; }
        public string Path { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
