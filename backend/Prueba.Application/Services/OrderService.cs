using AutoMapper;
using Microsoft.Extensions.Configuration;
using Prueba.Application.Dtos;
using Prueba.Application.Interfaces;
using Prueba.Domain.Entities;
using Prueba.Domain.Interfaces;
using System.Security.Authentication; 

namespace Prueba.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _blobBaseUrl;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _blobBaseUrl = configuration["BlobStorage:BaseUrl"];
        }

        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto dto, int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new AuthenticationException("Usuario no válido.");

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0
            };

 
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Producto ID {item.ProductId} no encontrado.");

             
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para {product.Name}.");

   
                product.Stock -= item.Quantity;
                _unitOfWork.Products.Update(product);

       
                var orderItem = new OrderItem
                {
                    Order = newOrder,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price 
                };
                newOrder.OrderItems.Add(orderItem);

        
                newOrder.TotalAmount += (item.Quantity * product.Price);
            }

          
            await _unitOfWork.Orders.AddAsync(newOrder);
            await _unitOfWork.CompleteAsync(); 
            return _mapper.Map<OrderDto>(newOrder);
        }

        public async Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

           
            foreach (var orderDto in orderDtos)
            {
                foreach (var itemDto in orderDto.OrderItems)
                {
                    
                    var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                    if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
                    {
                        itemDto.ImageUrl = $"{_blobBaseUrl}/{product.ImageUrl}";
                    }
                   
                }
            }

            return orderDtos;
        }
    }
}