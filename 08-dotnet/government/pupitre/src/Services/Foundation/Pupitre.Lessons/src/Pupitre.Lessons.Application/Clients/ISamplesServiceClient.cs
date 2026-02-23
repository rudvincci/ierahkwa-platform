using System;
using Mamey.Types;

namespace Pupitre.Lessons.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

