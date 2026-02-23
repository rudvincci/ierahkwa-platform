namespace Mamey.Mifos.Exceptions
{
    public class MifosBadRequestException : Exception
    {
        public MifosBadRequestException()
            : base($"Mifos server responded with bad request.")
        {
        }
    }
}

