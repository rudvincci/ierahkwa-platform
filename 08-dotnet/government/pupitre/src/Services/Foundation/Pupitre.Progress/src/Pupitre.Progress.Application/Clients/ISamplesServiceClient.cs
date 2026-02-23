using System;
using Mamey.Types;

namespace Pupitre.Progress.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

