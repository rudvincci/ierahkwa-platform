// using System;
// using System.Threading.Tasks;
// using Chronicle;
// using Mamey.ApplicationName.Modules.Saga.Api.Messages;
// using Mamey.ApplicationName.Modules.Saga.Api.Messages.BankApplications;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.Time;
//
// namespace Mamey.ApplicationName.Modules.Saga.Api.Sagas.Applications;
//
// internal sealed class ApplySagaData
// {
//     public DateTime VerifiedAt { get; set; }
//     public Guid WalletId { get; set; }
//     public string Currency { get; set; }
//     public bool DepositCompleted { get; set; }
// }
//
// internal sealed class ApplySaga : Saga<ApplySagaData>,
//     ISagaStartAction<Apply>
//     
// {
//     private const decimal BonusFunds = 10;
//     private const string TransferName = "new_customer_bonus";
//     private readonly IMessageBroker _messageBroker;
//     private readonly IClock _clock;
//
//     public ApplySaga(IMessageBroker messageBroker, IClock clock)
//     {
//         _messageBroker = messageBroker;
//         _clock = clock;
//     }
//
//     public override SagaId ResolveId(object message, ISagaContext context)
//         => message switch
//         {
//             Apply m => m.ApplicationId.ToString(),
//             DepositCompleted m => m.CustomerId.ToString(),
//             WalletAdded m => m.OwnerId.ToString(),
//             FundsAdded m => m.OwnerId.ToString(),
//             _ => base.ResolveId(message, context)
//         };
//
//     public async Task HandleAsync(Apply message, ISagaContext context)
//     {
//         ICommand? command = null;
//         if (message.AccountType is AccountType.Individual)
//         {
//             command = new ApplyIndividualMasterAccount();
//         }
//         else if(message.AccountType is AccountType.Corporate)
//         {
//             command = new ApplyCorporateMasterAccount();
//         }
//         await _messageBroker.PublishAsync(command);
//     }
//
//     public Task CompensateAsync(Apply message, ISagaContext context)
//     {
//         throw new NotImplementedException();
//     }
// }