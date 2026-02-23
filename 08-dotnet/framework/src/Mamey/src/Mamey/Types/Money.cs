using System.Globalization;

namespace Mamey.Types;

[Serializable]
public class Money : IEquatable<Money>
{
    private Money()
    {
        Amount = 0;
        Currency = new Currency("EUR");
    }
    public Amount Amount { get; }
    public Currency Currency { get; }

    [JsonConstructor]
    public Money(Amount amount, Currency currency)
    {
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public static Money Zero(string currencyCode) => new Money(Amount.Zero, new Currency(currencyCode));

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot add amounts with different currencies.");
        }

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot subtract amounts with different currencies.");
        }

        return new Money(a.Amount - b.Amount, a.Currency);
    }
    public static bool operator <(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount < b.Amount;
    }
    public static bool operator >(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount > b.Amount;
    }
    public static bool operator <=(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount <= b.Amount;
    }
    public static bool operator >=(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount >= b.Amount;
    }
    public static bool operator ==(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount == b.Amount;
    }
    public static bool operator !=(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot evaluate amounts with different currencies.");
        }

        return a.Amount == b.Amount;
    }

    public bool Equals(Money other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Money)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Amount.GetHashCode() * 397) ^ Currency.GetHashCode();
        }
    }

    public override string ToString()
    {
        var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(Currency.Code);
        return string.Format(culture, "{0:C}", Amount.Value);
    }
    public object ToSerializableObject()
    {
        return new { Amount = Amount.Value, Currency = Currency.Code };
    }
}

public static class MoneyExtensions
{
    public static Money ZeroMoney(Currency currency) => Money.Zero(currency);
}