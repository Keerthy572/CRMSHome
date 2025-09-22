namespace CRMSHome.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
    }
}
