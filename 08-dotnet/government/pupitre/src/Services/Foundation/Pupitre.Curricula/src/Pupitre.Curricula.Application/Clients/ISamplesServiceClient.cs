using System;
using Mamey.Types;

namespace Pupitre.Curricula.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

