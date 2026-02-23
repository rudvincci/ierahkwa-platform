using System.Text.Json.Serialization;

namespace Mamey.Mifos.Entities
{
    /// <summary>
    /// Clients are people and businesses that have applied (or may apply) to an MFI for loans.
    /// </summary>
    /// <remarks>Clients can be created in Pending or straight into Active state.</remarks>
    public class Client
    {
        public Client()
        {
        }
        public int Id { get; set; }

        /// <summary>
        /// If provided during client creation, its value is set as account number
        /// for client account, otherwise an auto generated account number is put
        /// in place based on the configured strategy.
        /// </summary>
        [JsonPropertyName("accountNo")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// A place to put an external reference for this client e.g. The ID another system uses.
        /// If provided, it must be unique.
        /// </summary>
        /// <remarks>If provided, it must be unique.</remarks>
        public string ExternalId { get; set; }


        /// <summary>
        /// Indicates whether this client is to be created as active client.
        /// If active=true, then activationDate must be provided.
        /// If active=false, then the client is created as pending.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The date on which the client became active.
        /// </summary>
        public DateTime ActivationDate { get; set; }

        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        [JsonPropertyName("middlename")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Facility to break up name into parts suitable for humans.
        /// </summary>
        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Facility to set name of a client or business that doesn't suit the
        /// firstname,middlename,lastname structure.
        /// </summary>
        [JsonPropertyName("fullname")]
        public string FullName { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Optional: unique mobile number that is used by SMS or Mobile Money functionality.
        /// </summary>
        [JsonPropertyName("mobileNo")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// The staffId of the staff member dealing with the client office.
        /// The staff member is not specifically the loan officer.
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        ///  Default overdraft savings account of client
        /// </summary>
        public string? SavingsProductId { get; set; }

        /// <summary>
        /// Facility to enrich client details.
        /// </summary>
        public string DataTables { get; set; }


        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
        public IEnumerable<OfficeOption> OfficeOptions { get; set; }
        public IEnumerable<StaffOption> StaffOptions { get; set; }
        public IEnumerable<SavingProductOption> SavingProductOptions { get; set; }
        public IEnumerable<DataTable> DataTable { get; set; }
        public IEnumerable<Option> GenderOptions { get; set; }
        public IEnumerable<ClientTypeOption> ClientTypeOptions { get; set; }
        public IEnumerable<ClientLegalFormOption> ClientLegalFormOptions { get; set; }

        // TODO: Still to be defined the type of obejct it will return. For now using an object type.

        public IEnumerable<object> ClientClassificationOptions { get; set; }
        public IEnumerable<object> ClientNonPersonConstitutionOptions { get; set; }
        public IEnumerable<object> ClientNonPersonMainBusinessLineOptions { get; set; }
        public bool IsAddressEnabled { get; set; }
        public Address Address { get; set; }


        //public static Client Create(int officeId, string firstName, string lastName,
        //    bool active = false, DateTime? activationDate = null)
        //{
            
        //}

        public void SetActive()
        {

        }
    }
}

