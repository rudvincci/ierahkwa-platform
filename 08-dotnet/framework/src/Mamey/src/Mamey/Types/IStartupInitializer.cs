namespace Mamey.Types;

public interface IStartupInitializer : IInitializer
{
    void AddInitializer(IInitializer initializer);
}
