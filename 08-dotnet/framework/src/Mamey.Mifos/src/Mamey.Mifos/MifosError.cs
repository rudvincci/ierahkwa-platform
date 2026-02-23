namespace Mamey.Mifos
{
    public class MifosError
    {
        public string DeveloperMessage { get; set; }
        public string DefaultUserMessage { get; set; }
        public string UserMessageGlobalisationCode { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public string[] Args { get; set; }
    }
}

