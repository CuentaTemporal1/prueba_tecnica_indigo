using Prueba.Application.Dtos;

namespace Prueba.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(int id);
        Task<PaginatedResultDto<ProductDto>> GetAllPaginatedAsync(PaginatedRequestDto request);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task UpdateAsync(int id, ProductUpdateDto dto);
        Task DeleteAsync(int id);
    }
}