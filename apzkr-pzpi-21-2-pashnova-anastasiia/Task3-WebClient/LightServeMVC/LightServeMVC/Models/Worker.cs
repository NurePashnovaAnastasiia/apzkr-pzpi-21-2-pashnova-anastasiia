namespace LightServeMVC.Models
{
    public class Worker : BaseEntity
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Cafe Cafe { get; set; }

        public int CafeId { get; set; }
    }
}
