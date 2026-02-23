using System;
using Mamey.Types;

namespace Pupitre.AIContent.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

