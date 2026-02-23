using Mamey.CQRS;
using Pupitre.AIVision.Domain.Entities;

namespace Pupitre.AIVision.Domain.Events;

internal record VisionAnalysisRemoved(VisionAnalysis VisionAnalysis) : IDomainEvent;