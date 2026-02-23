using System.Numerics;
using System.Text;

namespace Mamey.Auth.DecentralizedIdentifiers.Utilities;

/// <summary>
/// Utilities for Base58 encoding/decoding (Bitcoin alphabet).
/// </summary>
public static class Base58
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    private static readonly int[] Indexes = new int[128];

    static Base58()
    {
        for (int i = 0; i < Indexes.Length; i++) Indexes[i] = -1;
        for (int i = 0; i < Alphabet.Length; i++) Indexes[Alphabet[i]] = i;
    }

    /// <summary>
    /// Encodes a byte array to Base58 string.
    /// </summary>
    public static string Encode(byte[] input)
    {
        if (input == null || input.Length == 0) return "";

        var intData = new BigInteger(input.Concat(new byte[] { 0 }).ToArray()); // Append zero for BigInteger sign

        var sb = new StringBuilder();
        while (intData > 0)
        {
            int remainder = (int)(intData % 58);
            intData /= 58;
            sb.Insert(0, Alphabet[remainder]);
        }

        // Leading zero bytes
        foreach (var b in input)
        {
            if (b == 0) sb.Insert(0, Alphabet[0]);
            else break;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Decodes a Base58 string to bytes.
    /// </summary>
    public static byte[] Decode(string input)
    {
        if (string.IsNullOrEmpty(input)) return Array.Empty<byte>();

        BigInteger intData = 0;
        for (int i = 0; i < input.Length; i++)
        {
            int digit = input[i] < 128 ? Indexes[input[i]] : -1;
            if (digit < 0) throw new FormatException($"Invalid Base58 character `{input[i]}` at position {i}");
            intData = intData * 58 + digit;
        }

        // Leading zero bytes
        int leadingZeroCount = input.TakeWhile(c => c == Alphabet[0]).Count();
        var bytesWithoutSign = intData.ToByteArray(isUnsigned: true, isBigEndian: true);
        var result = new byte[leadingZeroCount + bytesWithoutSign.Length];
        Array.Copy(bytesWithoutSign, 0, result, leadingZeroCount, bytesWithoutSign.Length);
        return result;
    }
}