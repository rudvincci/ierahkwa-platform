//! Basic NFT Contract — Non-Fungible Token for MameyFutureNode.
//!
//! Supports:
//! - mint (minter only, assigns unique token IDs)
//! - transfer / transfer_from / approve
//! - owner_of / token_uri / balance_of / total_supply
//! - burn

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, math, validation};

// ─── Storage Keys ───────────────────────────────────────────────────────────

const KEY_NAME: &str = "nft:name";
const KEY_SYMBOL: &str = "nft:symbol";
const KEY_TOTAL_SUPPLY: &str = "nft:total_supply";
const KEY_NEXT_TOKEN_ID: &str = "nft:next_id";
const KEY_OWNER: &str = "nft:owner";
const KEY_MINTER: &str = "nft:minter";
const KEY_INITIALIZED: &str = "nft:initialized";

fn token_owner_key(token_id: u64) -> String { format!("nft:tok_owner:{}", token_id) }
fn token_uri_key(token_id: u64) -> String { format!("nft:tok_uri:{}", token_id) }
fn token_approval_key(token_id: u64) -> String { format!("nft:tok_appr:{}", token_id) }
fn balance_count_key(account: &str) -> String { format!("nft:bal:{}", account) }

// ─── Init ───────────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn init(name: String, symbol: String) -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Already initialized".into());
    }
    validation::validate_string_length(&name, 1, 100).map_err(|e| e.to_string())?;
    validation::validate_string_length(&symbol, 1, 20).map_err(|e| e.to_string())?;

    let caller = host_functions::get_caller();
    storage::set_string(KEY_NAME, &name).map_err(|e| e.to_string())?;
    storage::set_string(KEY_SYMBOL, &symbol).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_TOTAL_SUPPLY, 0).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_NEXT_TOKEN_ID, 1).map_err(|e| e.to_string())?;
    storage::set_string(KEY_OWNER, &caller).map_err(|e| e.to_string())?;
    storage::set_string(KEY_MINTER, &caller).map_err(|e| e.to_string())?;
    storage::set_bool(KEY_INITIALIZED, true).map_err(|e| e.to_string())?;

    host_functions::emit_event("NFTInitialized", &serde_json::json!({
        "name": name, "symbol": symbol, "owner": caller,
    }));
    Ok("NFT initialized".into())
}

// ─── Queries ────────────────────────────────────────────────────────────────

#[wasm_bindgen] pub fn name() -> String { storage::get_string(KEY_NAME).unwrap_or_default() }
#[wasm_bindgen] pub fn symbol() -> String { storage::get_string(KEY_SYMBOL).unwrap_or_default() }
#[wasm_bindgen] pub fn total_supply() -> u64 { storage::get_u64(KEY_TOTAL_SUPPLY).unwrap_or(0) }

#[wasm_bindgen]
pub fn owner_of(token_id: u64) -> Option<String> {
    storage::get_string(&token_owner_key(token_id))
}

#[wasm_bindgen]
pub fn token_uri(token_id: u64) -> Option<String> {
    storage::get_string(&token_uri_key(token_id))
}

#[wasm_bindgen]
pub fn balance_of(account: String) -> u64 {
    storage::get_u64(&balance_count_key(&account)).unwrap_or(0)
}

#[wasm_bindgen]
pub fn get_approved(token_id: u64) -> Option<String> {
    storage::get_string(&token_approval_key(token_id))
}

// ─── Mint ───────────────────────────────────────────────────────────────────

/// Mint a new NFT to `to` with the given metadata URI. Minter only.
#[wasm_bindgen]
pub fn mint(to: String, uri: String) -> Result<String, String> {
    require_minter()?;
    validation::validate_address(&to).map_err(|e| e.to_string())?;

    let token_id = storage::get_u64(KEY_NEXT_TOKEN_ID).unwrap_or(1);

    storage::set_string(&token_owner_key(token_id), &to).map_err(|e| e.to_string())?;
    storage::set_string(&token_uri_key(token_id), &uri).map_err(|e| e.to_string())?;

    let bal = storage::get_u64(&balance_count_key(&to)).unwrap_or(0);
    storage::set_u64(&balance_count_key(&to), math::safe_add(bal, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;

    let ts = total_supply();
    storage::set_u64(KEY_TOTAL_SUPPLY, math::safe_add(ts, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_NEXT_TOKEN_ID, math::safe_add(token_id, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;

    host_functions::emit_event("NFTMinted", &serde_json::json!({
        "token_id": token_id, "to": to, "uri": uri,
    }));
    Ok(format!("Minted token #{}", token_id))
}

// ─── Transfer ───────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn transfer(to: String, token_id: u64) -> Result<String, String> {
    let caller = host_functions::get_caller();
    let owner = storage::get_string(&token_owner_key(token_id))
        .ok_or("Token does not exist")?;

    if caller != owner {
        // Check approval
        let approved = storage::get_string(&token_approval_key(token_id));
        if approved.as_deref() != Some(&caller) {
            return Err("Not owner or approved".into());
        }
    }

    do_transfer(&owner, &to, token_id)?;

    host_functions::emit_event("NFTTransfer", &serde_json::json!({
        "from": owner, "to": to, "token_id": token_id,
    }));
    Ok("Transfer OK".into())
}

#[wasm_bindgen]
pub fn approve(to: String, token_id: u64) -> Result<String, String> {
    let caller = host_functions::get_caller();
    let owner = storage::get_string(&token_owner_key(token_id))
        .ok_or("Token does not exist")?;
    if caller != owner { return Err("Not the owner".into()); }

    storage::set_string(&token_approval_key(token_id), &to).map_err(|e| e.to_string())?;

    host_functions::emit_event("NFTApproval", &serde_json::json!({
        "owner": owner, "approved": to, "token_id": token_id,
    }));
    Ok("Approved".into())
}

// ─── Burn ───────────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn burn(token_id: u64) -> Result<String, String> {
    let caller = host_functions::get_caller();
    let owner = storage::get_string(&token_owner_key(token_id))
        .ok_or("Token does not exist")?;
    if caller != owner { return Err("Not the owner".into()); }

    // Remove token data
    host_functions::storage_del(&token_owner_key(token_id)).map_err(|e| e.to_string())?;
    host_functions::storage_del(&token_uri_key(token_id)).map_err(|e| e.to_string())?;
    host_functions::storage_del(&token_approval_key(token_id)).map_err(|e| e.to_string())?;

    // Decrement balance and supply
    let bal = storage::get_u64(&balance_count_key(&caller)).unwrap_or(1);
    storage::set_u64(&balance_count_key(&caller), math::safe_sub(bal, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;

    let ts = total_supply();
    storage::set_u64(KEY_TOTAL_SUPPLY, math::safe_sub(ts, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;

    host_functions::emit_event("NFTBurned", &serde_json::json!({
        "token_id": token_id, "owner": caller,
    }));
    Ok(format!("Token #{} burned", token_id))
}

// ─── Helpers ────────────────────────────────────────────────────────────────

fn do_transfer(from: &str, to: &str, token_id: u64) -> Result<(), String> {
    storage::set_string(&token_owner_key(token_id), to).map_err(|e| e.to_string())?;
    // Clear approval on transfer
    host_functions::storage_del(&token_approval_key(token_id)).map_err(|e| e.to_string())?;

    let from_bal = storage::get_u64(&balance_count_key(from)).unwrap_or(1);
    let to_bal = storage::get_u64(&balance_count_key(to)).unwrap_or(0);
    storage::set_u64(&balance_count_key(from), math::safe_sub(from_bal, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    storage::set_u64(&balance_count_key(to), math::safe_add(to_bal, 1).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    Ok(())
}

fn require_minter() -> Result<(), String> {
    let c = host_functions::get_caller();
    let m = storage::get_string(KEY_MINTER).ok_or("No minter")?;
    if c != m { return Err("Unauthorized".into()); }
    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_nft_init() {
        let r = init("MameyNFT".into(), "MNFT".into());
        assert!(r.is_ok());
        assert_eq!(name(), "MameyNFT");
        assert_eq!(symbol(), "MNFT");
        assert_eq!(total_supply(), 0);
    }
}
