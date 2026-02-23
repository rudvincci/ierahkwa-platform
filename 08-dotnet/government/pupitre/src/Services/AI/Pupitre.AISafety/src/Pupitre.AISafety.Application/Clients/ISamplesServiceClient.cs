using System;
using Mamey.Types;

namespace Pupitre.AISafety.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

