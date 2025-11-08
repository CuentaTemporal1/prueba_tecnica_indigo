using Microsoft.EntityFrameworkCore;
using Prueba.Domain.Common;
using Prueba.Domain.Entities;
using Prueba.Domain.Interfaces;
using Prueba.Infrastructure.Data;

namespace Prueba.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _context.Products.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return await query.AnyAsync(p => p.Name == name);
        }

        public async Task<PaginatedResult<Product>> GetPaginatedProductsAsync(int pageNumber, int pageSize, string searchTerm = null, string sortColumn = null, string sortOrder = "ASC")
        {
            var query = _context.Products.AsQueryable();

        
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm)
                                      || p.Description.Contains(searchTerm));
            }


            if (!string.IsNullOrWhiteSpace(sortColumn))
            {

                switch (sortColumn.ToLower())
                {
                    case "name":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query = sortOrder.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            return new PaginatedResult<Product>(items, totalCount);
        }
    }
}