namespace Mamey.Ntrada
{
    public interface IEnabledExtension
    {
        IExtension Extension { get; }
        IExtensionOptions Options { get; }
    }
}