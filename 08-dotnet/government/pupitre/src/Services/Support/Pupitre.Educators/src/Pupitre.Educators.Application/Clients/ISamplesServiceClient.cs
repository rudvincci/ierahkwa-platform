using System;
using Mamey.Types;

namespace Pupitre.Educators.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

