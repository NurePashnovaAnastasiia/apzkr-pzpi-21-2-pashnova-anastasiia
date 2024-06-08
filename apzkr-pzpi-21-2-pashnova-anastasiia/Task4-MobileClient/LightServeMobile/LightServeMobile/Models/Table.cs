namespace LightServeMobile.Models
{
    public class Table : BaseEntity
    {
        public int Number { get; set; }

        public int Size { get; set; }

        public bool IsAvailable { get; set; } = true;

        public Cafe Cafe { get; set; }

        public int CafeId { get; set; }
    }
}
