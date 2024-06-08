using System.ComponentModel.DataAnnotations;

namespace LightServeMVC.Models
{
    public class User
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        [Key]
        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsOwner { get; set; }

        public bool IsAuthorized { get; set; } = false;

        public Order? Order { get; set; }

        public Owner Owner { get; set; }

        public Customer Customer { get; set; }
    }
}
