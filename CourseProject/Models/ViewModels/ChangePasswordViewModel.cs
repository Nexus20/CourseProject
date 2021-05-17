using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models.ViewModels {
    public class ChangePasswordViewModel {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NewPassword { get; set; }
    }
}
