using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Products.Core.Events;
    public record BankingProductCreated(Guid Id) : IEvent;
    public record BankingProductUpdated(Guid Id) : IEvent;
    public record BankingProductDeleted(Guid Id) : IEvent;
