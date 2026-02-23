namespace Mamey.Mifos.Commands.Clients
{
    public class CreateClient : MifosCommand
    {
        public CreateClient(int officeId, string? firstname = null, string? lastname = null, string? fullname = null,
            bool active = true, DateTime? activationDate = null, int? groupId = null, string? externalId = null,
            string? accountNumber = null, int? staffId = null, string? mobileNo = null,
            int? savingsProductId = null, int? genderId = null, int? clientTypeId = null,
            int? clientClassififcationId = null, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            OfficeId = officeId;
            Firstname = firstname;
            Lastname = lastname;
            Fullname = fullname;
            Active = active;
            ActivationDate = activationDate.HasValue ? activationDate.Value.ToString(dateFormat) : string.Empty;
            GroupId = groupId;
            ExternalId = externalId;
            AccountNumber = accountNumber;
            StaffId = staffId;
            MobileNo = mobileNo;
            SavingsProductId = savingsProductId;
            GenderId = genderId;
            ClientTypeId = clientTypeId;
            ClientClassififcationId = clientClassififcationId;
            SubmittedOnDate = DateTime.Today.ToString(dateFormat);
        }

        public int OfficeId { get; }
        public string? Firstname { get; }
        public string? Lastname { get; }
        public string? Fullname { get; }
        public string? ExternalId { get; }
        public bool Active { get; }
        public string ActivationDate { get; }
        public int? GroupId { get; }
        public string? AccountNumber { get; }
        public int? StaffId { get; }
        public string? MobileNo { get; }
        public int? SavingsProductId { get; }
        public int? GenderId { get; }
        public int? ClientTypeId { get; }
        public int? ClientClassififcationId { get; }
        
        public string SubmittedOnDate { get; }
    }

}

