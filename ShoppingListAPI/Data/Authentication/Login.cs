using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Data.Authentication
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
