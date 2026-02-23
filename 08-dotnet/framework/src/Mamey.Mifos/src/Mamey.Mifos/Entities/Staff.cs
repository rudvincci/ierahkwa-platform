namespace Mamey.Mifos.Entities
{
    public class Staff
    {
        public Staff()
        {
        }
        public int Id { get; set; }
        public int Firstname { get; set; }
        public int Lastname { get; set; }
        public string DisplayName { get; set; }
        public bool IsLoanOfficer { get; set; }
        public string ExternalId { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public string JoiningDate { get; set; }
        public string Locale { get; set; }
        public string DateFormat { get; set; }
    }
}

