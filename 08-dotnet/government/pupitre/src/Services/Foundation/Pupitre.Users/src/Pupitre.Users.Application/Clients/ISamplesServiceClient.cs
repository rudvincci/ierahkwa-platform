using System;
using Mamey.Types;

namespace Pupitre.Users.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

