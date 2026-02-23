using System;
using Mamey.Types;

namespace Pupitre.Assessments.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

