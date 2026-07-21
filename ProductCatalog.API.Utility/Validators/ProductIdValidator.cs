using FluentValidation;

namespace ProductCatalog.API.Utility.Validators
{
    public class ProductIdValidator : AbstractValidator<int>
    {
       public ProductIdValidator()
       {
          RuleFor(id => id).GreaterThanOrEqualTo(0).WithMessage("Product id must be a Zero or positive integer.");
       }
    }
}
