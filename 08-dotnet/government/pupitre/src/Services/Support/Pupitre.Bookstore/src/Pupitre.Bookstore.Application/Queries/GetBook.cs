using System;
using Mamey.CQRS.Queries;
using Pupitre.Bookstore.Application.DTO;

namespace Pupitre.Bookstore.Application.Queries;

internal record GetBook(Guid Id) : IQuery<BookDetailsDto>;
