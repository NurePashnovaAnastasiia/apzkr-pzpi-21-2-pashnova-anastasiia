namespace LightServeMVC.Models.Dto
{
    public class UserDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

    }
}
