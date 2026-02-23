public interface IAccountInformation
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string TermsOfService { get; set; }
    public IAccountContact Contact { get; set; }
    public IAccountLicense AccountLicense { get; set;}
    public string Version { get; set; }  
}
