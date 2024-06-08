using System.ComponentModel.DataAnnotations;

namespace LightServeWebAPI.Models
{
    public class Owner
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        [Key]
        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<Cafe> Cafes { get; set; }
    }
}
