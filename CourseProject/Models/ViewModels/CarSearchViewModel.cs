namespace CourseProject.Models.ViewModels
{
    public class CarSearchViewModel : SearchViewModel
    {
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        public int[] FuelTypes { get; set; }
        public int[] BodyTypes { get; set; }
        public int[] TransmissionTypes { get; set; }
        public int[] CarStates { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public string[] Colors { get; set; }
    }
}
