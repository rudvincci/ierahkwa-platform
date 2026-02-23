using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.MethodBase
{
    /// <summary>
    /// Provides a base implementation for DID methods, supplying shared logic and structure.
    /// </summary>
    public abstract class DidMethodBase : IDidMethod
    {
        /// <summary>
        /// The method name (e.g. "web", "ion", "ethr", etc).
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Resolves a DID using the method's resolution logic.
        /// </summary>
        public abstract Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new DID Document (must be implemented in method subclass).
        /// </summary>
        public virtual Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException($"CreateAsync is not implemented for did:{Name}.");
        }

        /// <summary>
        /// Updates an existing DID Document (must be implemented in method subclass).
        /// </summary>
        public virtual Task<IDidDocument> UpdateAsync(string did, object updateRequest, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException($"UpdateAsync is not implemented for did:{Name}.");
        }

        /// <summary>
        /// Deactivates the specified DID (must be implemented in method subclass).
        /// </summary>
        public virtual Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException($"DeactivateAsync is not implemented for did:{Name}.");
        }

        /// <summary>
        /// Utility to validate the format and method name.
        /// </summary>
        protected void ValidateDid(string did)
        {
            if (string.IsNullOrWhiteSpace(did))
                throw new ArgumentNullException(nameof(did));
            var didObj = new Did(did);
            if (!string.Equals(didObj.Method, Name, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"DID '{did}' does not use method '{Name}'.");
        }
    }
}
