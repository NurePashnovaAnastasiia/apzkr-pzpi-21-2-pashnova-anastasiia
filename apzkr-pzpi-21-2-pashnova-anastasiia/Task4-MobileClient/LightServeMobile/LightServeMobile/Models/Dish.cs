namespace LightServeMobile.Models
{
    public class Dish : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public double Weight { get; set; }

        public Menu Menu { get; set; }

        public int MenuId { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
