namespace Mamey.Mifos.Queries.Staffs
{
    public class RetrieveStaffMember : IMifosQuery
    {
        public RetrieveStaffMember(int id)
        {
            Id = id;
        }
        public int Id { get; }
    }
}

