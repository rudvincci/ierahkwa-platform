namespace Mamey.Mifos.Exceptions
{
    public class MifosUnauthorizedException : Exception
	{
        public MifosUnauthorizedException()
            : base($"Not authorized to access Mifos server.")
        {
        }
    }
}

