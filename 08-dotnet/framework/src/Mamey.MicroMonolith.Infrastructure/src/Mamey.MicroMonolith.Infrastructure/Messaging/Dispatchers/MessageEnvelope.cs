using Mamey.MessageBrokers;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Dispatchers;

internal record MessageEnvelope(IMessage Message, IMessageContext MessageContext);