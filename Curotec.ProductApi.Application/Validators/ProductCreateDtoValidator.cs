using Curotec.ProductApi.Application.DTOs;
using FluentValidation;

namespace Curotec.ProductApi.Application.Validators;

public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}