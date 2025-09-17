namespace CRMSHome.Models
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string CarType { get; set; } = string.Empty;
        public int SeatCapacity { get; set; }
        public int RentPerDay { get; set; }
        public string AvailableStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;



    }
}
