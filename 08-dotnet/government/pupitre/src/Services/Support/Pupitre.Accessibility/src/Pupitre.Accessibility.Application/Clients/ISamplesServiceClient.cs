using System;
using Mamey.Types;

namespace Pupitre.Accessibility.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

