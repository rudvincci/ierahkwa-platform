namespace Mamey.Mifos.Entities
{
    public class StaffOptions
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DisplayName { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public bool IsLoanOfficer { get; set; }
        public bool IsActive { get; set; }
    }
}

