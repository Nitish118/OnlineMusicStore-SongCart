using System.ComponentModel.DataAnnotations;

namespace OnlineMusicStore.Models
{
    public class User
    {
        public int Id { get; set; }

        // Remove Required attribute from Username since you want to login with Email.
        // [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Select a payment method")]
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, or UPI

        public string CreditCard { get; set; }

        public string DebitCard { get; set; }

        public string UPI { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
