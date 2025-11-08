using System.Collections.Generic;

namespace Prueba.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } 
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}