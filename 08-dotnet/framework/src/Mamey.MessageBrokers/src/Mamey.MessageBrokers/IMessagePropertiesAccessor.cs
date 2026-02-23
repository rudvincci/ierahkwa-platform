namespace Mamey.MessageBrokers;

public interface IMessagePropertiesAccessor
{
    IMessageProperties MessageProperties { get; set; }
}