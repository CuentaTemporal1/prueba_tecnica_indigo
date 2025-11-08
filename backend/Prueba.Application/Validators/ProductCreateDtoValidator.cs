using FluentValidation;
using Prueba.Application.Dtos;
using Prueba.Domain.Interfaces;

namespace Prueba.Application.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductCreateDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("El nombre del producto ya existe.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("La imagen es requerida.");
        }
        private async Task<bool> BeUniqueName(string name, CancellationToken token)
        {
            return !await _unitOfWork.Products.ExistsByNameAsync(name);
        }
    }
}