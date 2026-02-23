using System;
using Mamey.Types;

namespace Pupitre.Analytics.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

