using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class CarModel {

        public int ID { get; set; }

        public string Name { get; set; }

        public int BrandID { get; set; }
        public Brand Brand { get; set; }

        public int ParentModelID { get; set; }
        public CarModel ParentModel { get; set; }

    }
}
