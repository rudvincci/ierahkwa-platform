using System;
using Mamey.Types;

namespace Pupitre.Parents.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

