using System;
using Mamey.Types;

namespace Pupitre.AIVision.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

