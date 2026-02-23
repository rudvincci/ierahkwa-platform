using System;
using System.Runtime.InteropServices;

namespace Mamey.Security.PostQuantum.Native;

/// <summary>
/// Thin P/Invoke wrapper around the Open Quantum Safe (liboqs) C library.
/// Provides ML-DSA (Dilithium) and ML-KEM (Kyber) primitives as described
/// in the Quantum Resistant MameyNode implementation plan.
/// </summary>
public static class LibOQS
{
    private const string LibraryName = "oqs";

    // ---------------------------------------------------------------------
    // ML-DSA (Dilithium) - ML-DSA-44/65/87
    // ---------------------------------------------------------------------

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_2_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_2_sign(
        byte[] signature,
        ref ulong signature_len,
        byte[] message,
        ulong message_len,
        byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_2_verify(
        byte[] message,
        ulong message_len,
        byte[] signature,
        ulong signature_len,
        byte[] public_key);

    // Constants for Dilithium level 2 (ML-DSA-44) from the plan
    public const int OQS_SIG_dilithium_2_length_public_key = 1312;
    public const int OQS_SIG_dilithium_2_length_secret_key = 2528;
    public const int OQS_SIG_dilithium_2_length_signature = 2420;

    // ML-DSA-65 (Dilithium 3)
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_3_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_3_sign(
        byte[] signature,
        ref ulong signature_len,
        byte[] message,
        ulong message_len,
        byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_3_verify(
        byte[] message,
        ulong message_len,
        byte[] signature,
        ulong signature_len,
        byte[] public_key);

    public const int OQS_SIG_dilithium_3_length_public_key = 1952;
    public const int OQS_SIG_dilithium_3_length_secret_key = 4000;
    public const int OQS_SIG_dilithium_3_length_signature = 3293;

    // ML-DSA-87 (Dilithium 5)
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_5_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_5_sign(
        byte[] signature,
        ref ulong signature_len,
        byte[] message,
        ulong message_len,
        byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_SIG_dilithium_5_verify(
        byte[] message,
        ulong message_len,
        byte[] signature,
        ulong signature_len,
        byte[] public_key);

    public const int OQS_SIG_dilithium_5_length_public_key = 2592;
    public const int OQS_SIG_dilithium_5_length_secret_key = 4864;
    public const int OQS_SIG_dilithium_5_length_signature = 4595;

    // ---------------------------------------------------------------------
    // ML-KEM (Kyber) - ML-KEM-512/768/1024
    // ---------------------------------------------------------------------

    // ML-KEM-512 (Kyber 512)
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_512_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_512_encaps(
        byte[] ciphertext,
        byte[] shared_secret,
        byte[] public_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_512_decaps(
        byte[] shared_secret,
        byte[] ciphertext,
        byte[] secret_key);

    public const int OQS_KEM_kyber_512_length_public_key = 800;
    public const int OQS_KEM_kyber_512_length_secret_key = 1632;
    public const int OQS_KEM_kyber_512_length_ciphertext = 768;
    public const int OQS_KEM_kyber_512_length_shared_secret = 32;

    // ML-KEM-768 (Kyber 768)
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_768_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_768_encaps(
        byte[] ciphertext,
        byte[] shared_secret,
        byte[] public_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_768_decaps(
        byte[] shared_secret,
        byte[] ciphertext,
        byte[] secret_key);

    // Kyber-768 constants from the plan (can be verified against liboqs docs)
    public const int OQS_KEM_kyber_768_length_public_key = 1184;
    public const int OQS_KEM_kyber_768_length_secret_key = 2400;
    public const int OQS_KEM_kyber_768_length_ciphertext = 1088;
    public const int OQS_KEM_kyber_768_length_shared_secret = 32;

    // ML-KEM-1024 (Kyber 1024)
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_1024_keypair(byte[] public_key, byte[] secret_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_1024_encaps(
        byte[] ciphertext,
        byte[] shared_secret,
        byte[] public_key);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OQS_KEM_kyber_1024_decaps(
        byte[] shared_secret,
        byte[] ciphertext,
        byte[] secret_key);

    public const int OQS_KEM_kyber_1024_length_public_key = 1568;
    public const int OQS_KEM_kyber_1024_length_secret_key = 3168;
    public const int OQS_KEM_kyber_1024_length_ciphertext = 1568;
    public const int OQS_KEM_kyber_1024_length_shared_secret = 32;

    // Helper to translate liboqs status codes into .NET exceptions.
    public static void EnsureSuccess(int status, string operation)
    {
        if (status != 0)
        {
            throw new InvalidOperationException($"liboqs operation '{operation}' failed with status {status}.");
        }
    }
}


