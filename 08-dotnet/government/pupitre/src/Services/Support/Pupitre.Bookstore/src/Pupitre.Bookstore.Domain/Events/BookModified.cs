using Mamey.CQRS;
using Pupitre.Bookstore.Domain.Entities;

namespace Pupitre.Bookstore.Domain.Events;

internal record BookModified(Book Book): IDomainEvent;

