using Prueba.Application.Dtos;
namespace Prueba.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(OrderCreateDto dto, int userId);
        Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int userId);
    }
}