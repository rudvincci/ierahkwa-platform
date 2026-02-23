using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Aftercare.Domain.Entities;

namespace Pupitre.Aftercare.Application.Exceptions;

internal class AftercarePlanNotFoundException : MameyException
{
    public AftercarePlanNotFoundException(AftercarePlanId aftercareplanId)
        : base($"AftercarePlan with ID: '{aftercareplanId.Value}' was not found.")
        => AftercarePlanId = aftercareplanId;

    public AftercarePlanId AftercarePlanId { get; }
}

