using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Ministries.Domain.Entities;

namespace Pupitre.Ministries.Application.Exceptions;

internal class MinistryDataNotFoundException : MameyException
{
    public MinistryDataNotFoundException(MinistryDataId ministrydataId)
        : base($"MinistryData with ID: '{ministrydataId.Value}' was not found.")
        => MinistryDataId = ministrydataId;

    public MinistryDataId MinistryDataId { get; }
}

