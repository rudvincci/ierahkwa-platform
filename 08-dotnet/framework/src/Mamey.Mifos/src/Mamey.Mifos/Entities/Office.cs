namespace Mamey.Mifos.Entities
{
    /// <summary>
    /// Offices are used to model an MFIs structure.
    /// A hierarchical representation of offices is supported.
    /// There will always be at least one office (which represents the MFI or an MFIs head office).
    /// All subsequent offices added must have a parent office.
    /// </summary>
    public class Office
    {
        public Office()
        {
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string NameDecorated { get; set; }
        public string ExternalId { get; set; }
        public string[] OpeningDate { get; set; }
        public string Hierarchy { get; set; }
    }
}

