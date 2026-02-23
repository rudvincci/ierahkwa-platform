using Mamey.CQRS;
using Pupitre.Bookstore.Domain.Entities;

namespace Pupitre.Bookstore.Domain.Events;

internal record BookRemoved(Book Book) : IDomainEvent;