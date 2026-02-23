using System;
using Mamey.Types;

namespace Pupitre.AITutors.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

