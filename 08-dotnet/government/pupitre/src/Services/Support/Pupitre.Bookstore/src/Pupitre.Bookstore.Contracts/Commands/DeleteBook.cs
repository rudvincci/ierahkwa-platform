using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Bookstore.Contracts.Commands;

[Contract]
public record DeleteBook(Guid Id) : ICommand;


