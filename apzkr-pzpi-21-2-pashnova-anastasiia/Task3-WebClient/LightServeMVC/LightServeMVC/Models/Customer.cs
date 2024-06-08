using System.ComponentModel.DataAnnotations;

namespace LightServeMVC.Models
{
    public class Customer 
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        [Key]
        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
