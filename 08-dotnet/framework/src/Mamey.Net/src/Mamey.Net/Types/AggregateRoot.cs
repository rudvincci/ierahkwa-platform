// using System.Linq.Expressions;
// using Mamey.Exceptions;
// using Mamey.Types;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.Net.Types;
//
// public abstract class AggregateRoot<T>
// {
//     private bool _versionIncremented;
//     private readonly List<IDomainEvent> _events = new();
//     protected AggregateRoot() { }
//
//     public AggregateRoot(T id, int version = 0)
//     {
//         Id = id;
//         Version = version;
//     }
//
//     [PersonalData]
//     public virtual T Id { get; protected set; }
//
//     public virtual int Version { get; protected set; } = 1;
//     [JsonIgnore]
//     public IEnumerable<IDomainEvent> Events => _events;
//
//     public void AddEvent(IDomainEvent @event)
//     {
//         if (!_events.Any() && !_versionIncremented)
//         {
//             Version++;
//             _versionIncremented = true;
//         }
//
//         _events.Add(@event);
//     }
//
//     public void ClearEvents() => _events.Clear();
//
//     public void IncrementVersion()
//     {
//         if (_versionIncremented)
//         {
//             return;
//         }
//
//         Version++;
//         _versionIncremented = true;
//     }
//
//     public (bool, List<ValidationResult>?) Validate(bool throwException = true)
//     {
//         List<ValidationResult> results;
//         if (AggregateRoot<T>.TryValidate(this, out results, throwException))
//         {
//             return (true, null);
//         };
//         return (false, results);
//     }
//
//     protected static bool TryValidate<TEntity>(AggregateRoot<TEntity> aggregateRoot,
// out List<ValidationResult> results, bool throwException = true)
//     {
//         results = new List<ValidationResult>();
//         var validationContext = new ValidationContext(aggregateRoot);
//
//         var valid = Validator.TryValidateObject(aggregateRoot, validationContext, results);
//
//         if (throwException)
//         {
//             var result = results?.FirstOrDefault();
//             if (result is not null)
//                 throw new InvalidAggregatePropertyException<TEntity>(result?.MemberNames.FirstOrDefault(), result?.ErrorMessage);
//         }
//
//         return valid;
//     }
//
//     protected bool TryValidateProperty<TEntity>(object propertyValue, Expression<Func<TEntity, object>> predicate, out List<ValidationResult> results, bool throwException = true)
//     {
//         results = new List<ValidationResult>();
//         var propertyName = predicate.GetMemberName();
//         var validationContext = new ValidationContext(this) { MemberName = propertyName };
//         var valid = Validator.TryValidateProperty(propertyValue, validationContext, results);
//
//         if (!valid && throwException)
//         {
//             var result = results?.FirstOrDefault();
//             throw new InvalidAggregatePropertyException<TEntity>(predicate?.Parameters.FirstOrDefault()?.Type.Name, result?.ErrorMessage);
//         }
//
//         return valid;
//     }
// }
//
// public abstract class AggregateRoot : AggregateRoot<AggregateId>
// {
//     protected AggregateRoot(AggregateId id, int version = 0) : base(id, version) { }
// }
//
