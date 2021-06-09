using System.ComponentModel.DataAnnotations;

namespace CourseProject.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
    }
}
