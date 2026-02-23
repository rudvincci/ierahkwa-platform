using Mamey.Mifos.Entities;

namespace Mamey.Mifos
{
    public interface IMifosClient
    {
        public int Id { get; set; }
        /// <summary>
        /// If provided during client creation, its value is set as account no.
        /// for client account, otherwise an auto generated account no. is put
        /// in place based on the configured strategy.
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// A place to put an external reference for this client
        /// e.g. The ID another system uses. If provided, it must be unique.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Indicates whether this client is to be created as active client.
        /// If active=true, then activationDate must be provided. If
        /// active=false, then the client is created as pending.
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// The date on which the client became active.
        /// </summary>
        public DateTime ActivationDate { get; set; }
        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        public string Fistname { get; set; }
        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        public string Middlename { get; set; }
        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        public string Lastname { get; set; }
        /// <summary>
        /// Facility to set name of a client or business that doesn't suit the
        /// firstname,middlename,lastname structure.
        /// </summary>
        public string Fullname { get; set; }
        /// <summary>
        /// Optional: unique mobile number that is used by SMS or Mobile Money functionality.
        /// </summary>
        public string? MobileNo { get; set; }
        /// <summary>
        /// The staffId of the staff member dealing with the client office.
        /// The staff member is not specifically the loan officer.
        /// </summary>
        public string StaffId { get; set; }
        /// <summary>
        /// Facility to enrich client details.
        /// </summary>
        public Dictionary<string, object> Datatables { get; set; }
        public OfficeOptions OfficeOptions { get; set; }
        public StaffOptions StaffOptions { get; set; }
        public SavingProductOptions SavingProductOptions { get; set; }
    }
}

