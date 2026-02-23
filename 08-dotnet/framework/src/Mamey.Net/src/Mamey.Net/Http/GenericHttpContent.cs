using System.Net;
using System.Text;
using System.Text.Json;

namespace Mamey.Net.Http;

public class GenericHttpContent<T> : HttpContent
{
    private readonly T _content;
    private readonly Encoding _encoding;

    public GenericHttpContent(T content, Encoding encoding = null)
    {
        _content = content;
        _encoding = encoding ?? Encoding.UTF8;
        Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        using (var writer = new StreamWriter(stream, _encoding))
        {
            var json = JsonSerializer.Serialize(_content);
            return writer.WriteAsync(json);
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}


