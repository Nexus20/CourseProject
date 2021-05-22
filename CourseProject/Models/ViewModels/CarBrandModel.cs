﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models.ViewModels {
    public class CarBrandModel {

        public int ModelId { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }

        public string ModelNameWithBrand => $"{BrandName} {ModelName}";

    }
}
