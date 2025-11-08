using Microsoft.EntityFrameworkCore;
using Prueba.Domain.Entities;
using Prueba.Domain.Interfaces;
using Prueba.Infrastructure.Data;

namespace Prueba.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) 
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}