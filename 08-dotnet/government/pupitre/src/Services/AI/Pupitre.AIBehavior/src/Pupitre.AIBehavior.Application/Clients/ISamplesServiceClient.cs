using System;
using Mamey.Types;

namespace Pupitre.AIBehavior.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

