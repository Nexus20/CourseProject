using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class PurchaseRequest {

        public int Id { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
