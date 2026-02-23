using System;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

