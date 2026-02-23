namespace Mamey.Mifos.Entities
{
    public enum LoanType
    {
        /// <summary>
        /// Loan given to individual member.
        /// </summary>
        Individual,
        /// <summary>
        /// Loan given to group as a whole
        /// </summary>
        Group,
        /// <summary>
        /// Joint liability group loan given to members in a group on individual
        /// basis. JLG loan can be given to one or more members in a group.
        /// </summary>
        JointLiabilityGroup
    }
}

