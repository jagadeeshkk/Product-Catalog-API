using ProductCatalog.API.Utility.Validators;

namespace ProductCatalog.API.Test
{
    [TestFixture]
    public class ProductIdValidatorTests
    {
        private ProductIdValidator _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _sut = new ProductIdValidator();
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(100)]
        public void Validate_ZeroOrPositiveId_IsValid(int id)
        {
            var result = _sut.Validate(id);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase(-1)]
        [TestCase(-100)]
        public void Validate_NegativeId_IsInvalid(int id)
        {
            var result = _sut.Validate(id);
    
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
        }
    }
}
