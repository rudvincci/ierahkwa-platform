using System;
using Mamey.Types;

namespace Pupitre.AIAssessments.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

