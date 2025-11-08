using AutoMapper;
using Microsoft.Extensions.Configuration;
using Prueba.Application.Dtos;
using Prueba.Application.Interfaces;
using Prueba.Domain.Common;
using Prueba.Domain.Entities;
using Prueba.Domain.Interfaces;

namespace Prueba.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;
        private readonly string _blobBaseUrl;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IBlobStorageService blobStorageService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _blobStorageService = blobStorageService;
            _blobBaseUrl = configuration["BlobStorage:BaseUrl"];
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            string path = $"products/{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            await _blobStorageService.UploadAsync(dto.Image.OpenReadStream(), path, dto.Image.ContentType);
            product.ImageUrl = path; 

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.ImageUrl = _blobStorageService.GetFileUrl(product.ImageUrl);

            return productDto;
        }

        public async Task<PaginatedResultDto<ProductDto>> GetAllPaginatedAsync(PaginatedRequestDto request)
        {
            var paginatedResult = await _unitOfWork.Products.GetPaginatedProductsAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.SortColumn,
                request.SortOrder
            );

            var resultDto = _mapper.Map<PaginatedResultDto<ProductDto>>(paginatedResult);
          foreach (var item in resultDto.Items)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageUrl = _blobStorageService.GetFileUrl(item.ImageUrl);
                }
            }
            return resultDto;
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return null;

            var productDto = _mapper.Map<ProductDto>(product);
            if (!string.IsNullOrEmpty(productDto.ImageUrl))
            {
                productDto.ImageUrl = _blobStorageService.GetFileUrl(product.ImageUrl);
            }
            return productDto;
        }

        public async Task UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Producto no encontrado");
            
            _mapper.Map(dto, product);

            if (dto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _blobStorageService.DeleteAsync(product.ImageUrl);
                }

                string path = $"products/{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                await _blobStorageService.UploadAsync(dto.Image.OpenReadStream(), path, dto.Image.ContentType);
                product.ImageUrl = path;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Producto no encontrado");

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _blobStorageService.DeleteAsync(product.ImageUrl);
            }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}