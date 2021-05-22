using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CourseProject.Models.ViewModels {
    public class CarSearchViewModel : SearchViewModel {

        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public int[] FuelTypes { get; set; }
        public int[] BodyTypes { get; set; }
        public int[] TransmissionTypes { get; set; }
        public int[] CarStates { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }

    }
}
