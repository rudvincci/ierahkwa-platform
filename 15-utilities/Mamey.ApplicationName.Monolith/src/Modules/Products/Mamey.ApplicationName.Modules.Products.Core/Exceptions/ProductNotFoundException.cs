using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Products.Core.Exceptions
{
    public class ProductNotFoundException : MameyException
    {
        public Guid Id { get; }

        public ProductNotFoundException(Guid id) : base($"Product with id '{id} was not found.'")
            => Id = id;
    }
}