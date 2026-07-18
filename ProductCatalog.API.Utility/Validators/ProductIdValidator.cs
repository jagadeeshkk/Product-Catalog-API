using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

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
