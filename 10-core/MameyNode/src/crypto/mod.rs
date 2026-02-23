//! Cryptographic utilities for MameyNode

use aes_gcm::{Aes256Gcm, Key, Nonce, aead::{Aead, KeyInit}};
use chacha20poly1305::ChaCha20Poly1305;
use sha2::{Sha256, Sha512, Digest};
use sha3::{Keccak256, Sha3_256};
use ed25519_dalek::{Signer, SigningKey, VerifyingKey, Signature};
use rand::rngs::OsRng;
use argon2::{Argon2, password_hash::{SaltString, PasswordHasher}};

pub mod keys;
pub mod signatures;
pub mod zkp;

/// Hash data using Keccak256 (Ethereum compatible)
pub fn keccak256(data: &[u8]) -> [u8; 32] {
    let mut hasher = Keccak256::new();
    hasher.update(data);
    hasher.finalize().into()
}

/// Hash data using SHA256
pub fn sha256(data: &[u8]) -> [u8; 32] {
    let mut hasher = Sha256::new();
    hasher.update(data);
    hasher.finalize().into()
}

/// Hash data using SHA512
pub fn sha512(data: &[u8]) -> [u8; 64] {
    let mut hasher = Sha512::new();
    hasher.update(data);
    hasher.finalize().into()
}

/// Hash data using SHA3-256
pub fn sha3_256(data: &[u8]) -> [u8; 32] {
    let mut hasher = Sha3_256::new();
    hasher.update(data);
    hasher.finalize().into()
}

/// Generate a random 32-byte key
pub fn generate_random_bytes(len: usize) -> Vec<u8> {
    use rand::RngCore;
    let mut bytes = vec![0u8; len];
    OsRng.fill_bytes(&mut bytes);
    bytes
}

/// Encrypt data using AES-256-GCM
pub fn aes_encrypt(key: &[u8; 32], nonce: &[u8; 12], plaintext: &[u8]) -> Result<Vec<u8>, String> {
    let key = Key::<Aes256Gcm>::from_slice(key);
    let cipher = Aes256Gcm::new(key);
    let nonce = Nonce::from_slice(nonce);
    
    cipher.encrypt(nonce, plaintext)
        .map_err(|e| format!("Encryption failed: {}", e))
}

/// Decrypt data using AES-256-GCM
pub fn aes_decrypt(key: &[u8; 32], nonce: &[u8; 12], ciphertext: &[u8]) -> Result<Vec<u8>, String> {
    let key = Key::<Aes256Gcm>::from_slice(key);
    let cipher = Aes256Gcm::new(key);
    let nonce = Nonce::from_slice(nonce);
    
    cipher.decrypt(nonce, ciphertext)
        .map_err(|e| format!("Decryption failed: {}", e))
}

/// Encrypt data using ChaCha20-Poly1305
pub fn chacha_encrypt(key: &[u8; 32], nonce: &[u8; 12], plaintext: &[u8]) -> Result<Vec<u8>, String> {
    let key = chacha20poly1305::Key::from_slice(key);
    let cipher = ChaCha20Poly1305::new(key);
    let nonce = chacha20poly1305::Nonce::from_slice(nonce);
    
    cipher.encrypt(nonce, plaintext)
        .map_err(|e| format!("Encryption failed: {}", e))
}

/// Decrypt data using ChaCha20-Poly1305
pub fn chacha_decrypt(key: &[u8; 32], nonce: &[u8; 12], ciphertext: &[u8]) -> Result<Vec<u8>, String> {
    let key = chacha20poly1305::Key::from_slice(key);
    let cipher = ChaCha20Poly1305::new(key);
    let nonce = chacha20poly1305::Nonce::from_slice(nonce);
    
    cipher.decrypt(nonce, ciphertext)
        .map_err(|e| format!("Decryption failed: {}", e))
}

/// Hash password using Argon2
pub fn hash_password(password: &str) -> Result<String, String> {
    let salt = SaltString::generate(&mut OsRng);
    let argon2 = Argon2::default();
    
    argon2.hash_password(password.as_bytes(), &salt)
        .map(|hash| hash.to_string())
        .map_err(|e| format!("Password hashing failed: {}", e))
}

/// Generate Ed25519 keypair
pub fn generate_keypair() -> (SigningKey, VerifyingKey) {
    let signing_key = SigningKey::generate(&mut OsRng);
    let verifying_key = signing_key.verifying_key();
    (signing_key, verifying_key)
}

/// Sign message with Ed25519
pub fn sign_message(signing_key: &SigningKey, message: &[u8]) -> Signature {
    signing_key.sign(message)
}

/// Verify Ed25519 signature
pub fn verify_signature(verifying_key: &VerifyingKey, message: &[u8], signature: &Signature) -> bool {
    verifying_key.verify_strict(message, signature).is_ok()
}

/// Derive address from public key (Ethereum-style)
pub fn derive_address(public_key: &[u8]) -> String {
    let hash = keccak256(public_key);
    format!("0x{}", hex::encode(&hash[12..]))
}

#[cfg(test)]
mod tests {
    use super::*;
    
    #[test]
    fn test_aes_encryption() {
        let key = [0u8; 32];
        let nonce = [0u8; 12];
        let plaintext = b"Hello, Ierahkwa!";
        
        let ciphertext = aes_encrypt(&key, &nonce, plaintext).unwrap();
        let decrypted = aes_decrypt(&key, &nonce, &ciphertext).unwrap();
        
        assert_eq!(plaintext.to_vec(), decrypted);
    }
    
    #[test]
    fn test_chacha_encryption() {
        let key = [0u8; 32];
        let nonce = [0u8; 12];
        let plaintext = b"Hello, Ierahkwa!";
        
        let ciphertext = chacha_encrypt(&key, &nonce, plaintext).unwrap();
        let decrypted = chacha_decrypt(&key, &nonce, &ciphertext).unwrap();
        
        assert_eq!(plaintext.to_vec(), decrypted);
    }
    
    #[test]
    fn test_ed25519_signature() {
        let (signing_key, verifying_key) = generate_keypair();
        let message = b"Test message";
        
        let signature = sign_message(&signing_key, message);
        assert!(verify_signature(&verifying_key, message, &signature));
    }
}
