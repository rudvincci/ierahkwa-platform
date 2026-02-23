using System;
using System.Collections.Generic;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages.ProductManagement;

public record ProductCreated(Guid ProductId, string Name, string Description, string Category, List<string> Images) : IEvent;
