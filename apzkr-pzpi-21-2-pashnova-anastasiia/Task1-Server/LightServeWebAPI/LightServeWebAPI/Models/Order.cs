namespace LightServeWebAPI.Models
{
    public class Order : BaseEntity
    {
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public bool IsDone { get; set; } = false;

        public Cafe Cafe { get; set; }

        public int CafeId { get; set; }

        public Customer Customer { get; set; }  

        public string CustomerEmail { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
