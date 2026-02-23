using System;
using Mamey.Types;

namespace Pupitre.AISpeech.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

