namespace Mamey.Ntrada.Extensions.RabbitMq
{
    public interface IContextBuilder
    {
        object Build(ExecutionData executionData);
    }
}