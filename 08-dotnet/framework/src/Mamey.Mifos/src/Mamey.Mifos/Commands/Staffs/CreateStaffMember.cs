namespace Mamey.Mifos.Commands.Staffs
{
    public class CreateStaffMember : IMifosCommand
    {
        public CreateStaffMember(int officeId, string firstname, string lastname,
            string externalId, string mobileNo, bool isLoanOfficer, bool isActive,
            string joiningDate, string dateFormat, string locale)
        {
            OfficeId = officeId;
            Firstname = firstname;
            Lastname = lastname;
            ExternalId = externalId;
            MobileNo = mobileNo;
            IsLoanOfficer = isLoanOfficer;
            IsActive = isActive;
            JoiningDate = joiningDate;
            DateFormat = dateFormat;
            Locale = locale;
        }
        /// <summary>
        /// Office Id
        /// </summary>
        public int OfficeId { get; private set; }
        /// <summary>
        /// First Name of the new Employee.
        /// </summary>
        public string Firstname { get; private set; }
        /// <summary>
        /// Last Name of the new Employee.
        /// </summary>
        public string Lastname { get; private set; }
        /// <summary>
        /// ID to put an external reference for an Employee.
        /// </summary>
        public string ExternalId { get; private set; }
        /// <summary>
        /// Mobile number of an Employee.
        /// </summary>
        public string MobileNo { get; private set; }
        /// <summary>
        /// Indicates whether the employee account is to be created as Loan Officer.
        /// If isLoanOfficer=true, then the employee is Loan Officer.
        /// If isLoanOfficer=false, then the employee is not Loan Officer.
        /// </summary>
        public bool IsLoanOfficer { get; private set; }
        /// <summary>
        /// Indicates whether the employee account is to be created as Active.
        /// If isActive=true, then employee is active.
        /// If isActive=false, then employee is inactive.
        /// </summary>
        public bool IsActive { get; private set; }
        public string JoiningDate { get; private set; }
        public string DateFormat { get; private set; }
        public string Locale { get; private set; }
    }
}

