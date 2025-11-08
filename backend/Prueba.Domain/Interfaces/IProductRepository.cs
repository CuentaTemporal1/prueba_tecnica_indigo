using Prueba.Domain.Common; // Importamos la clase de paginación
using Prueba.Domain.Entities;
using System.Threading.Tasks;

namespace Prueba.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
  
        Task<PaginatedResult<Product>> GetPaginatedProductsAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string sortColumn = null,
            string sortOrder = "ASC");

        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}