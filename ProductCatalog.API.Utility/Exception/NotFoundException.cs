using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.API.Utility.Exception
{
    public class NotFoundException : IOException
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public static NotFoundException ForProduct(int id) =>
            new($"Product with id {id} was not found.");
    }
}
