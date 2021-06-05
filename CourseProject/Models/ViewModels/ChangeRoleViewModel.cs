using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Models.ViewModels {
    public class ChangeRoleViewModel {

        public string UserId { get; set; }
        [Display(Name = "Login")]
        public string UserName { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel() {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }

    }
}
