namespace Mamey.Mifos
{
    public class MifosErrorResult
    {
        public string DeveloperMessage { get; set; }
        public string DeveloperDocLink { get; set; }
        public string HttpStatusCode { get; set; }
        public string DefaultUserMessage { get; set; }
        public string UserMessageGlobalisationCode { get; set; }
        IEnumerable<MifosError> Errors { get; set; }
        
    }
}

