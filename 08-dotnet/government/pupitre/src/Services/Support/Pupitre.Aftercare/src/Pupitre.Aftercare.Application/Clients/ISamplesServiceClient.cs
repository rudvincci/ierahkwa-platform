using System;
using Mamey.Types;

namespace Pupitre.Aftercare.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

