namespace Prueba.Application.Dtos
{

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; } 
        public List<string> Roles { get; set; }
    }
}