namespace Mamey.Mifos.Entities
{
    public record Timeline(DateTime submittedOnDate, string SubmittedByUsername,
        string SubmittedByFirstName, string SubmittedByLastName, DateTime ActivatedOnDate, string activatedByUsername);
}

