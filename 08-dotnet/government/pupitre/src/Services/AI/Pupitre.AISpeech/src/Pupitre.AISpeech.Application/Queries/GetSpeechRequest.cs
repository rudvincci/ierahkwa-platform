using System;
using Mamey.CQRS.Queries;
using Pupitre.AISpeech.Application.DTO;

namespace Pupitre.AISpeech.Application.Queries;

internal record GetSpeechRequest(Guid Id) : IQuery<SpeechRequestDetailsDto>;
