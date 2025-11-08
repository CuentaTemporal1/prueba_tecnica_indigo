using Prueba.Domain.Entities;
using System.Collections.Generic;

namespace Prueba.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}