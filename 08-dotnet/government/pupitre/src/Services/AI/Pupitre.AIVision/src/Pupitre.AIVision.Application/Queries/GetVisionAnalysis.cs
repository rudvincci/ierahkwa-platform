using System;
using Mamey.CQRS.Queries;
using Pupitre.AIVision.Application.DTO;

namespace Pupitre.AIVision.Application.Queries;

internal record GetVisionAnalysis(Guid Id) : IQuery<VisionAnalysisDetailsDto>;
