using System;
using Mamey.Types;

namespace Pupitre.AITranslation.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

