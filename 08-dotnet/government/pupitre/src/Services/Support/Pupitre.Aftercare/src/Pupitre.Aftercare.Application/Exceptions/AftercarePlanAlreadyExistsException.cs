using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Aftercare.Domain.Entities;

namespace Pupitre.Aftercare.Application.Exceptions;

internal class AftercarePlanAlreadyExistsException : MameyException
{
    public AftercarePlanAlreadyExistsException(AftercarePlanId aftercareplanId)
        : base($"AftercarePlan with ID: '{aftercareplanId.Value}' already exists.")
        => AftercarePlanId = aftercareplanId;

    public AftercarePlanId AftercarePlanId { get; }
}
