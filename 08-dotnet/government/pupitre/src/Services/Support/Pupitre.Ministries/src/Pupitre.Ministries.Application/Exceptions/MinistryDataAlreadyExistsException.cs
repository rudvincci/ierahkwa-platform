using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Ministries.Domain.Entities;

namespace Pupitre.Ministries.Application.Exceptions;

internal class MinistryDataAlreadyExistsException : MameyException
{
    public MinistryDataAlreadyExistsException(MinistryDataId ministrydataId)
        : base($"MinistryData with ID: '{ministrydataId.Value}' already exists.")
        => MinistryDataId = ministrydataId;

    public MinistryDataId MinistryDataId { get; }
}
