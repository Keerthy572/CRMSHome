using System.ComponentModel.DataAnnotations;

namespace CRMSHome.Models
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "Cardholder name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters are allowed")]
        [Display(Name = "Cardholder Name")]
        public string CardHolder { get; set; }

       
       
        [Required(ErrorMessage = "Card number is required")]
        [RegularExpression(@"^(\d{4}\s?){4}$", ErrorMessage = "Card number must be 16 digits")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }





        [Required(ErrorMessage = "Expiry date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry must be in MM/YY format")]
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
        [DataType(DataType.Password)]
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
    }
}
