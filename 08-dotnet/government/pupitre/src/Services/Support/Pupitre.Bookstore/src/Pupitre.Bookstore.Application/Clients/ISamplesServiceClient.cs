using System;
using Mamey.Types;

namespace Pupitre.Bookstore.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

