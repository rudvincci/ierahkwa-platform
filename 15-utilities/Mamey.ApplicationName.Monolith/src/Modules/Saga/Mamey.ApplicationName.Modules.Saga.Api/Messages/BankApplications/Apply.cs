// using System;
// using Mamey.CQRS.Commands;
// using Mamey.Types;
//
// namespace Mamey.ApplicationName.Modules.Saga.Api.Messages.BankApplications;
//
// public class Apply : ICommand
// {
//     public Guid ApplicationId { get; set; } = Guid.NewGuid();
//     public Name SettlorName { get; set; }
//     public DateTime AppliedOn { get; set; }
//     public AccountType AccountType { get; set; } // Personal or Corporate
//     public MasterTrustType TrustType { get; set; } // Revocable or Irrevocable
//     public string Email { get; set; } = string.Empty;
//     public Phone PhoneNumber { get; set; }
//     public string Username { get; set; } = string.Empty;
//     public string Password { get; set; } = string.Empty;
//     public bool AgreeTerms { get; set; } = false;
// }
//
// public class  ApplyIndividualMasterAccount : ICommand
// {
//     public MasterTrustType TrustType { get; set; } // Revocable or Irrevocable
// }
// public class ApplyCorporateMasterAccount : ICommand
// {
//     public MasterTrustType TrustType { get; set; } // Revocable or Irrevocable
// }