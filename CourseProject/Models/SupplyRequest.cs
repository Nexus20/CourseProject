using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models {
    public class SupplyRequest {

        public int Id { get; set; }

        [Required]
        public int DealerId { get; set; }
        public Dealer Dealer { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public int Count { get; set; }

        public enum SupplyRequestState {
            New,
            Sent,
            Closed
        }

        public SupplyRequestState State { get; set; }

        public SupplyRequest() {
            State = SupplyRequestState.New;
        }

    }
}
