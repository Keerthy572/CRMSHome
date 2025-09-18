namespace CRMSHome.ViewModels
{
    public class AllBookingsViewModel
    {
        public string CustomerFullName { get; set; }
        public string CustomerUsername { get; set; }
        public string CustomerEmail { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public int CarSeatCapacity { get; set; }
        public int CarRentPerDay { get; set; }
        public string CarImagePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsActive { get; set; }
    }
}
