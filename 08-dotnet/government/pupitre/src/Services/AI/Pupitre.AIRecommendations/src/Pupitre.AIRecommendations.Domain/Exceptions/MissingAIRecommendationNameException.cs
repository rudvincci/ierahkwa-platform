using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIRecommendations.Domain.Exceptions;

internal class MissingAIRecommendationNameException : DomainException
{
    public MissingAIRecommendationNameException()
        : base("AIRecommendation name is missing.")
    {
    }

    public override string Code => "missing_airecommendation_name";
}
