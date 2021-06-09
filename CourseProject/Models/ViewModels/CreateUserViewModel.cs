using System.ComponentModel.DataAnnotations;

namespace CourseProject.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Display(Name = "Login")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
