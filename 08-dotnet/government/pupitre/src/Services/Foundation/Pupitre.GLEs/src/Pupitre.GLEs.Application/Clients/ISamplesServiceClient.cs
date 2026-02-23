using System;
using Mamey.Types;

namespace Pupitre.GLEs.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

