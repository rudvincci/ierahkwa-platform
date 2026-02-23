using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Products.Core.Exceptions
{
    public sealed class ProductAlreadyExistsException : MameyException
    {
        public Guid Id { get; }
        public string Name { get; }

        public ProductAlreadyExistsException(Guid id) : base($"Product with id: '{id}' already exists.")
            => Id = id;

        public ProductAlreadyExistsException(string name) : base($"Product with name: '{name}' already exists.")
            => Name = name;
    }
    
}