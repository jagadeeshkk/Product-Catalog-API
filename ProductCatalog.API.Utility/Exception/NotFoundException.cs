namespace ProductCatalog.API.Utility.Exception
{
    public class NotFoundException : global::System.Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public static NotFoundException ForProduct(int id) =>
            new($"Product with id {id} was not found.");
    }
    public class ServiceUnavailableException : global::System.Exception
    {
        public ServiceUnavailableException(string message, global::System.Exception innerException) : base(message, innerException) { }
    }

    public class MetricCalculationException : global::System.Exception
    {
        public MetricCalculationException(string message, global::System.Exception innerException) : base(message, innerException) { }
    }
}
