namespace LightServeWebAPI.Models.Dto
{
    public class PopularDishDto
    {
        public int? DishId { get; set; }
        public string DishName { get; set; }
        public int OrderCount { get; set; }
    }
}
