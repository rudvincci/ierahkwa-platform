using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Casino.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in the system.
/// Immutable; supports basic arithmetic operations.
/// </summary>
[Keyless]
public sealed class Money : IEquatable<Money>
{
    private Money(){} // For EF
    /// <summary>
    /// Gets the amount in the smallest currency unit (e.g., dollars).
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Money"/> with the specified amount.
    /// </summary>
    /// <param name="amount">The monetary amount; must be non-negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="amount"/> is negative.</exception>
    public Money(decimal amount)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Money amount cannot be negative.");
        Amount = amount;
    }

    /// <summary>
    /// Adds two <see cref="Money"/> values.
    /// </summary>
    public Money Add(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        return new Money(Amount + other.Amount);
    }

    /// <summary>
    /// Subtracts the specified <see cref="Money"/> from this instance.
    /// </summary>
    public Money Subtract(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        var result = Amount - other.Amount;
        if (result < 0m)
            throw new InvalidOperationException("Resulting Money cannot be negative.");
        return new Money(result);
    }

    /// <summary>
    /// Multiplies this <see cref="Money"/> by a scalar multiplier.
    /// </summary>
    /// <param name="multiplier">Non-negative multiplier.</param>
    /// <returns>New <see cref="Money"/> with scaled amount.</returns>
    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0m)
            throw new ArgumentOutOfRangeException(nameof(multiplier), "Multiplier cannot be negative.");
        return new Money(Amount * multiplier);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as Money);

    /// <inheritdoc/>
    public bool Equals(Money other) =>
        other != null && Amount == other.Amount;

    /// <inheritdoc/>
    public override int GetHashCode() => Amount.GetHashCode();

    /// <summary>
    /// Returns a string representation of the monetary amount.
    /// </summary>
    public override string ToString() => Amount.ToString("F2");
    /// <summary>
    /// Implicit conversion from <see cref="UserId"/> to <see cref="Guid"/>.
    /// </summary>
    /// <param name="amount">The user ID.</param>
    public static implicit operator decimal(Money amount) => amount.Amount;

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/> to <see cref="UserId"/>.
    /// </summary>
    /// <param name="amount">The GUID value.</param>
    public static implicit operator Money(decimal amount) => new(amount);
}


/// <summary>
/// Represents the identifier for a game, encapsulating a GUID.
/// </summary>
public readonly struct GameId : IEquatable<GameId>
{
    /// <summary>
    /// Gets the underlying GUID value of this GameId.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameId"/> struct.
    /// </summary>
    /// <param name="value">The GUID value. Must not be <see cref="Guid.Empty"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public GameId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GameId cannot be an empty GUID.", nameof(value));
        Value = value;
    }

    /// <inheritdoc/>
    public bool Equals(GameId other) => Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        obj is GameId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of the underlying GUID.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicit conversion from <see cref="Guid"/> to <see cref="GameId"/>.
    /// </summary>
    public static implicit operator GameId(Guid value) => new GameId(value);

    /// <summary>
    /// Implicit conversion from <see cref="GameId"/> to <see cref="Guid"/>.
    /// </summary>
    public static implicit operator Guid(GameId id) => id.Value;
}


/// <summary>
/// Represents a payout multiplier as a value object.
/// </summary>
public readonly struct Multiplier : IEquatable<Multiplier>
{
    /// <summary>
    /// Gets the multiplier value (must be non-negative).
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Multiplier"/> struct.
    /// </summary>
    /// <param name="value">Multiplier value; must be ≥ 0.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative.</exception>
    public Multiplier(double value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Multiplier cannot be negative.");
        Value = value;
    }

    /// <inheritdoc/>
    public bool Equals(Multiplier other) => Value.Equals(other.Value);

    /// <inheritdoc/>
    public override bool Equals(object obj) =>
        obj is Multiplier other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of the multiplier.
    /// </summary>
    public override string ToString() => Value.ToString("F2");
}

