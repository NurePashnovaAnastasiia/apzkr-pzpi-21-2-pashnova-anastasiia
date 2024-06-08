namespace LightServeMobile.Models
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}
