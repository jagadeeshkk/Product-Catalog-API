using NUnit.Framework.Internal;
using ProductCatalog.API.Utility.Exception;

namespace ProductCatalog.API.Test
{
    [TestFixture]
    public class NotFoundExceptionTests
    {
        [Test]
        public void ForProduct_BuildsMessageContainingId()
        {
            var ex = NotFoundException.ForProduct(42);
            Assert.That(ex.Message, Does.Contain("42"));
        }
    }
}
