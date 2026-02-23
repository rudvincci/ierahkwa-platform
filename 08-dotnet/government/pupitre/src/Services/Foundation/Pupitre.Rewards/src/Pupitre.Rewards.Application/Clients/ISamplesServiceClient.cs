using System;
using Mamey.Types;

namespace Pupitre.Rewards.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

