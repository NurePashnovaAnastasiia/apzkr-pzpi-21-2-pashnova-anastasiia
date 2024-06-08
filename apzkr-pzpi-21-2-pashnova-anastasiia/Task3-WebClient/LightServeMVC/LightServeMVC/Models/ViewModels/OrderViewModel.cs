using System.Collections.Generic;
using static LightServeMVC.Models.ViewModels.OrderViewModel;

namespace LightServeMVC.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DishViewModel Dish { get; set; }
        public decimal Amount { get; set; }
    }
    public class DishViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
