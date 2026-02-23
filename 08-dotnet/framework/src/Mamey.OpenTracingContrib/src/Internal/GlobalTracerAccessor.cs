using OpenTracing;
using OpenTracing.Util;

namespace Mamey.OpenTracingContrib.Internal
{
    public class GlobalTracerAccessor : IGlobalTracerAccessor
    {
        public ITracer GetGlobalTracer()
        {
            return GlobalTracer.Instance;
        }
    }
}
