namespace LightServeMobile.Models
{
    public class OrderDetails : BaseEntity
    {
        public Order Order { get; set; }

        public int OrderId { get; set; }

        public Dish? Dish { get; set; }

        public int? DishId { get; set; }

        public int Amount { get; set; }
    }
}
