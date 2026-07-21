namespace ProductCatalog.API.Utility.Exception
{
    public class NotFoundException : System.Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public static NotFoundException ForProduct(int id) =>
            new($"Product with id {id} was not found.");
    }
    public class ServiceUnavailableException : System.Exception
    {
        public ServiceUnavailableException(string message, System.Exception innerException) : base(message, innerException) { }
    }

    public class MetricCalculationException : System.Exception
    {
        public MetricCalculationException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
