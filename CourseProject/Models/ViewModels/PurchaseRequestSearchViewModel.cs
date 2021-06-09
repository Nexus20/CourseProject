namespace CourseProject.Models.ViewModels
{
    public class PurchaseRequestSearchViewModel : SearchViewModel
    {
        public int? Id { get; set; }
        public int? CarAvailable { get; set; }
        public string Owner { get; set; }
        public int[] RequestStates { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}
