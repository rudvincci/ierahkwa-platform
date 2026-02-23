namespace Mamey.Tracing.Jaeger.Builders;

internal sealed class JaegerOptionsBuilder : IJaegerOptionsBuilder
{
    private readonly JaegerOptions _options = new();

    public IJaegerOptionsBuilder Enable(bool enabled)
    {
        _options.Enabled = enabled;
        return this;
    }

    public IJaegerOptionsBuilder WithServiceName(string serviceName)
    {
        _options.ServiceName = serviceName;
        return this;
    }

    public IJaegerOptionsBuilder WithUdpHost(string udpHost)
    {
        _options.UdpHost = udpHost;
        return this;
    }

    public IJaegerOptionsBuilder WithUdpPort(int udpPort)
    {
        _options.UdpPort = udpPort;
        return this;
    }

    public IJaegerOptionsBuilder WithMaxPacketSize(int maxPacketSize)
    {
        _options.MaxPacketSize = maxPacketSize;
        return this;
    }

    public IJaegerOptionsBuilder WithSampler(string sampler)
    {
        _options.Sampler = sampler;
        return this;
    }

    public IJaegerOptionsBuilder WithMaxTracesPerSecond(double maxTracesPerSecond)
    {
        _options.MaxTracesPerSecond = maxTracesPerSecond;
        return this;
    }

    public IJaegerOptionsBuilder WithSamplingRate(double samplingRate)
    {
        _options.SamplingRate = samplingRate;
        return this;
    }

    public JaegerOptions Build()
    {
        // Enable Jaeger by default if service name is provided and not explicitly disabled
        if (!_options.Enabled && !string.IsNullOrEmpty(_options.ServiceName))
        {
            _options.Enabled = true;
        }
        
        // Set default sender type if not specified
        if (string.IsNullOrWhiteSpace(_options.Sender))
        {
            _options.Sender = "udp";
        }
        
        // Set default UDP configuration if using UDP sender
        if (_options.Sender.Equals("udp", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(_options.UdpHost))
            {
                _options.UdpHost = "localhost";
            }
            
            if (_options.UdpPort <= 0)
            {
                _options.UdpPort = 6831;
            }
        }
        
        // Initialize HttpSender with default values if not set
        if (_options.HttpSender == null)
        {
            _options.HttpSender = new JaegerOptions.HttpSenderOptions();
        }
        return _options;
    }
}