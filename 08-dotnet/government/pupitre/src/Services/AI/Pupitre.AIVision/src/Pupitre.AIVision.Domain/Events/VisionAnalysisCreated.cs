using Mamey.CQRS;
using Pupitre.AIVision.Domain.Entities;

namespace Pupitre.AIVision.Domain.Events;

internal record VisionAnalysisCreated(VisionAnalysis VisionAnalysis) : IDomainEvent;

