using System.ComponentModel.DataAnnotations;

namespace CRMSHome.Models
{
    public class Admin
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password {get; set;} = string.Empty;
       
    }
}
