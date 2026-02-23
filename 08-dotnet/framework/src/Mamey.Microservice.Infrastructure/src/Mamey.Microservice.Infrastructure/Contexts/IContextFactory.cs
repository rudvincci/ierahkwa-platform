using Mamey.Contexts;

namespace Mamey.Microservice.Infrastructure.Contexts
{
    internal interface IContextFactory
    {
        IContext Create();
    }
}

