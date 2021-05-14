using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class CarModel {

        public int ID { get; set; }

        public string Name { get; set; }

        public int BrandID { get; set; }
        public Brand Brand { get; set; }

        [ForeignKey("Parent")]
        public int? ParentModelID { get; set; }
        public virtual CarModel Parent { get; set; }
        public virtual ICollection<CarModel> Children { get; set; }

    }
}
