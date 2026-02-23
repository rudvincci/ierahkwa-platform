namespace Mamey.Web3.Messages;

public class UrlChanged
{
    public UrlChanged(string url)
    {
        Url = url;
    }

    public string Url { get; }
}