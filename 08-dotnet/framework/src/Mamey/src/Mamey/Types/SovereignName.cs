// using System.Text.Json.Serialization;
//
// using Mamey.Types;
//
// namespace Mamey.Types;
//
// /// <summary>
// /// Represents a sovereign name with an optional trademark.
// /// </summary>
// [Serializable]
// public class SovereignName : Name, IEquatable<SovereignName>, ICloneable
// {
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="SovereignName"/> class.
//     /// </summary>
//     /// <param name="name">The base name.</param>
//     /// <param name="tradeMark">The optional trademark.</param>
//     public SovereignName(Name name, string? tradeMark)
//         : this(name?.FirstName ?? throw new ArgumentNullException(nameof(name.FirstName)),
//                string.IsNullOrEmpty(name?.LastName) ? "NLN" : name.LastName,
//                tradeMark, name.MiddleName, name.Nickname)
//     {
//     }
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="SovereignName"/> class with specified names.
//     /// </summary>
//     /// <param name="firstName">The first name.</param>
//     /// <param name="lastName">The last name.</param>
//     /// <param name="tradeMark">The optional trademark.</param>
//     /// <param name="middleName">The optional middle name.</param>
//     /// <param name="nickname">The optional nickname.</param>
//     [JsonConstructor]
//     public SovereignName(string firstName, string? lastName, string? tradeMark, string? middleName = null, string? nickname = null)
//         : base(firstName, string.IsNullOrEmpty(lastName) ? "NLN" : lastName, middleName, nickname)
//     {
//         TradeMark = tradeMark;
//         if (!string.IsNullOrEmpty(tradeMark))
//         {
//             AKA = (Name)base.Clone();
//             FirstName = tradeMark;
//             MiddleName = null;
//             LastName = "NLN";
//         }
//         else
//         {
//             TradeMark = FullName;
//         }
//     }
//
//     /// <summary>
//     /// Gets the trademark.
//     /// </summary>
//     public string? TradeMark { get; }
//
//     /// <summary>
//     /// Gets the alternative name.
//     /// </summary>
//     [ExcelExportIgnore]
//     public Name? AKA { get; }
//
//     /// <summary>
//     /// Gets the full name with or without the trademark.
//     /// </summary>
//     public override string FullName =>
//         string.IsNullOrEmpty(TradeMark)
//         ?
//             $"{FirstName} {(string.IsNullOrEmpty(MiddleName) ? "" : $"{MiddleName} ")}{LastName}"
//         :
//             $"{TradeMark}{(string.IsNullOrEmpty(AKA?.FullName) ? string.Empty :  $", aka {AKA?.FullName}")}";
//
//     /// <summary>
//     /// Gets the "also known as" name.
//     /// </summary>
//     public string AlsoKnownAs => AKA?.FullName ?? string.Empty;
//
//     /// <summary>
//     /// Gets the given names with or without the trademark.
//     /// </summary>
//     public override string GivenNames => string.IsNullOrEmpty(TradeMark) ? base.GivenNames : TradeMark;
//
//     /// <summary>
//     /// Gets the initials with or without the trademark.
//     /// </summary>
//     public override string Initials => string.IsNullOrEmpty(TradeMark)
//         ? base.Initials
//         : TradeMark[0].ToString();
//
//     /// <summary>
//     /// Determines whether the specified <see cref="SovereignName"/> is equal to the current <see cref="SovereignName"/>.
//     /// </summary>
//     /// <param name="other">The <see cref="SovereignName"/> to compare with the current <see cref="SovereignName"/>.</param>
//     /// <returns>true if the specified <see cref="SovereignName"/> is equal to the current <see cref="SovereignName"/>; otherwise, false.</returns>
//     public bool Equals(SovereignName? other)
//     {
//         if (other is null) return false;
//         if (ReferenceEquals(this, other)) return true;
//         return base.Equals(other) && TradeMark == other.TradeMark;
//     }
//
//     /// <summary>
//     /// Determines whether the specified object is equal to the current <see cref="SovereignName"/>.
//     /// </summary>
//     /// <param name="obj">The object to compare with the current <see cref="SovereignName"/>.</param>
//     /// <returns>true if the specified object is equal to the current <see cref="SovereignName"/>; otherwise, false.</returns>
//     public override bool Equals(object? obj)
//     {
//         return obj is SovereignName other && Equals(other);
//     }
//
//     /// <summary>
//     /// Serves as a hash function for the <see cref="SovereignName"/>.
//     /// </summary>
//     /// <returns>A hash code for the current <see cref="SovereignName"/>.</returns>
//     public override int GetHashCode()
//     {
//         unchecked // Allow overflow, ignore overflow for simplicity
//         {
//             int hash = base.GetHashCode();
//             hash = hash * 23 + (TradeMark != null ? TradeMark.GetHashCode() : 0);
//             return hash;
//         }
//     }
//
//     /// <summary>
//     /// Determines whether two instances of <see cref="SovereignName"/> are equal.
//     /// </summary>
//     /// <param name="a">The first <see cref="SovereignName"/> to compare.</param>
//     /// <param name="b">The second <see cref="SovereignName"/> to compare.</param>
//     /// <returns>true if the two instances of <see cref="SovereignName"/> are equal; otherwise, false.</returns>
//     public static bool operator ==(SovereignName? a, SovereignName? b)
//     {
//         if (ReferenceEquals(a, b)) return true;
//         if (a is null || b is null) return false;
//         return a.Equals(b);
//     }
//
//     /// <summary>
//     /// Determines whether two instances of <see cref="SovereignName"/> are not equal.
//     /// </summary>
//     /// <param name="a">The first <see cref="SovereignName"/> to compare.</param>
//     /// <param name="b">The second <see cref="SovereignName"/> to compare.</param>
//     /// <returns>true if the two instances of <see cref="SovereignName"/> are not equal; otherwise, false.</returns>
//     public static bool operator !=(SovereignName? a, SovereignName? b) => !(a == b);
//
//     /// <summary>
//     /// Returns a string that represents the current <see cref="SovereignName"/>.
//     /// </summary>
//     /// <returns>A string that represents the current <see cref="SovereignName"/>.</returns>
//     public override string ToString() => LastName == "NLN"? TradeMark : FullName;
//
//     /// <summary>
//     /// Returns a string that represents the current <see cref="SovereignName"/> with the alternative name.
//     /// </summary>
//     /// <returns>A string that represents the current <see cref="SovereignName"/> with the alternative name.</returns>
//     public string ToStringWithAKA() => $"{FullName}, aka {AlsoKnownAs}";
//
//     /// <summary>
//     /// Creates a new object that is a copy of the current instance.
//     /// </summary>
//     /// <returns>A new object that is a copy of this instance.</returns>
//     public new object Clone()
//     {
//         return new SovereignName(this, TradeMark);
//     }
// }