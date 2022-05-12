using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Data.Authentication
{
    public class Register
    {
        [Required]
        public string UserName { get; set; }
        
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
