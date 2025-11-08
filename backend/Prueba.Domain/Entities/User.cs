using System.Collections.Generic;

namespace Prueba.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}