namespace Mamey.Mifos.Commands.Clients
{
    public class AssignStaff : MifosCommand
    {
        public AssignStaff(int staffId, string dateFormat = "dd MMMM yyyy", string locale = "en")
            : base(dateFormat, locale)
        {
            StaffId = staffId;
        }
        public int StaffId { get; set; }
    }

}

