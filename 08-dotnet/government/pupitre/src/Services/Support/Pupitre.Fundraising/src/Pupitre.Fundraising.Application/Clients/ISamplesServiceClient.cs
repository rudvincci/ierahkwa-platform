using System;
using Mamey.Types;

namespace Pupitre.Fundraising.Application.Clients;

internal interface ISamplesServiceClient
{
    Task<SampleDto?> GetAsync(Guid id);
}

