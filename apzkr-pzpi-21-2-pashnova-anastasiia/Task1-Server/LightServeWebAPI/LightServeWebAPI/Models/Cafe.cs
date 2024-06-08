namespace LightServeWebAPI.Models
{
    public class Cafe : BaseEntity
    {
        public string Name { get; set; }

        public Owner Owner { get; set; }

        public string OwnerEmail { get; set; }

        public ICollection<Table> Tables { get; set; }

        public ICollection<Worker> Workers { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
