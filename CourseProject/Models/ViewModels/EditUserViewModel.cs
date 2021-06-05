using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseProject.Models.ViewModels {
    public class EditUserViewModel {
        public string Id { get; set; }
        [Display(Name = "Login")]
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
