using System;
using Mamey.Types;

namespace Pupitre.Notifications.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

