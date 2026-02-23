//! ERC-20 Token — Standard fungible token for MameyFutureNode.
//!
//! Full ERC-20-like implementation with:
//! - mint / burn
//! - transfer / transfer_from / approve
//! - balance_of / allowance / total_supply
//! - pause / unpause governance

use wasm_bindgen::prelude::*;
use mamey_contracts_shared::{host_functions, storage, math, validation};

// ─── Storage ────────────────────────────────────────────────────────────────

const KEY_NAME: &str = "token:name";
const KEY_SYMBOL: &str = "token:symbol";
const KEY_DECIMALS: &str = "token:decimals";
const KEY_TOTAL_SUPPLY: &str = "token:total_supply";
const KEY_OWNER: &str = "token:owner";
const KEY_MINTER: &str = "token:minter";
const KEY_PAUSED: &str = "token:paused";
const KEY_INITIALIZED: &str = "token:initialized";

fn balance_key(acct: &str) -> String { format!("bal:{}", acct) }
fn allowance_key(owner: &str, spender: &str) -> String { format!("allow:{}:{}", owner, spender) }

// ─── Init ───────────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn init(name: String, symbol: String, decimals: u8, initial_supply: u64) -> Result<String, String> {
    if storage::get_bool(KEY_INITIALIZED) {
        return Err("Already initialized".into());
    }
    validation::validate_string_length(&name, 1, 100).map_err(|e| e.to_string())?;
    validation::validate_string_length(&symbol, 1, 20).map_err(|e| e.to_string())?;

    let caller = host_functions::get_caller();
    storage::set_string(KEY_NAME, &name).map_err(|e| e.to_string())?;
    storage::set_string(KEY_SYMBOL, &symbol).map_err(|e| e.to_string())?;
    storage::set_u8(KEY_DECIMALS, decimals).map_err(|e| e.to_string())?;
    storage::set_u64(KEY_TOTAL_SUPPLY, initial_supply).map_err(|e| e.to_string())?;
    storage::set_string(KEY_OWNER, &caller).map_err(|e| e.to_string())?;
    storage::set_string(KEY_MINTER, &caller).map_err(|e| e.to_string())?;
    storage::set_bool(KEY_PAUSED, false).map_err(|e| e.to_string())?;
    storage::set_bool(KEY_INITIALIZED, true).map_err(|e| e.to_string())?;

    if initial_supply > 0 {
        storage::set_u64(&balance_key(&caller), initial_supply).map_err(|e| e.to_string())?;
    }

    host_functions::emit_event("TokenInitialized", &serde_json::json!({
        "name": name, "symbol": symbol, "decimals": decimals,
        "initial_supply": initial_supply, "owner": caller,
    }));
    Ok("Token initialized".into())
}

// ─── Queries ────────────────────────────────────────────────────────────────

#[wasm_bindgen] pub fn name() -> String { storage::get_string(KEY_NAME).unwrap_or_default() }
#[wasm_bindgen] pub fn symbol() -> String { storage::get_string(KEY_SYMBOL).unwrap_or_default() }
#[wasm_bindgen] pub fn decimals() -> u8 { storage::get_u8(KEY_DECIMALS).unwrap_or(18) }
#[wasm_bindgen] pub fn total_supply() -> u64 { storage::get_u64(KEY_TOTAL_SUPPLY).unwrap_or(0) }

#[wasm_bindgen]
pub fn balance_of(account: String) -> u64 {
    storage::get_u64(&balance_key(&account)).unwrap_or(0)
}

#[wasm_bindgen]
pub fn allowance(owner: String, spender: String) -> u64 {
    storage::get_u64(&allowance_key(&owner, &spender)).unwrap_or(0)
}

// ─── Transfer ───────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn transfer(to: String, amount: u64) -> Result<String, String> {
    require_not_paused()?;
    validation::validate_address(&to).map_err(|e| e.to_string())?;
    validation::validate_amount(amount).map_err(|e| e.to_string())?;

    let from = host_functions::get_caller();
    do_transfer(&from, &to, amount)?;

    host_functions::emit_event("Transfer", &serde_json::json!({"from": from, "to": to, "amount": amount}));
    Ok("Transfer OK".into())
}

#[wasm_bindgen]
pub fn transfer_from(from: String, to: String, amount: u64) -> Result<String, String> {
    require_not_paused()?;
    let spender = host_functions::get_caller();
    let curr = storage::get_u64(&allowance_key(&from, &spender)).unwrap_or(0);
    if curr < amount { return Err("Insufficient allowance".into()); }

    do_transfer(&from, &to, amount)?;

    let new_allow = math::safe_sub(curr, amount).map_err(|e| e.to_string())?;
    storage::set_u64(&allowance_key(&from, &spender), new_allow).map_err(|e| e.to_string())?;

    host_functions::emit_event("Transfer", &serde_json::json!({"from": from, "to": to, "amount": amount}));
    Ok("TransferFrom OK".into())
}

#[wasm_bindgen]
pub fn approve(spender: String, amount: u64) -> Result<String, String> {
    require_not_paused()?;
    let owner = host_functions::get_caller();
    storage::set_u64(&allowance_key(&owner, &spender), amount).map_err(|e| e.to_string())?;
    host_functions::emit_event("Approval", &serde_json::json!({"owner": owner, "spender": spender, "amount": amount}));
    Ok("Approved".into())
}

// ─── Mint / Burn ────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn mint(to: String, amount: u64) -> Result<String, String> {
    require_not_paused()?;
    require_minter()?;
    let bal = storage::get_u64(&balance_key(&to)).unwrap_or(0);
    storage::set_u64(&balance_key(&to), math::safe_add(bal, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    let ts = total_supply();
    storage::set_u64(KEY_TOTAL_SUPPLY, math::safe_add(ts, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    host_functions::emit_event("Mint", &serde_json::json!({"to": to, "amount": amount}));
    Ok("Minted".into())
}

#[wasm_bindgen]
pub fn burn(amount: u64) -> Result<String, String> {
    require_not_paused()?;
    let from = host_functions::get_caller();
    let bal = storage::get_u64(&balance_key(&from)).unwrap_or(0);
    if bal < amount { return Err("Insufficient balance".into()); }
    storage::set_u64(&balance_key(&from), math::safe_sub(bal, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    let ts = total_supply();
    storage::set_u64(KEY_TOTAL_SUPPLY, math::safe_sub(ts, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    host_functions::emit_event("Burn", &serde_json::json!({"from": from, "amount": amount}));
    Ok("Burned".into())
}

// ─── Governance ─────────────────────────────────────────────────────────────

#[wasm_bindgen]
pub fn pause() -> Result<String, String> {
    require_owner()?;
    storage::set_bool(KEY_PAUSED, true).map_err(|e| e.to_string())?;
    Ok("Paused".into())
}

#[wasm_bindgen]
pub fn unpause() -> Result<String, String> {
    require_owner()?;
    storage::set_bool(KEY_PAUSED, false).map_err(|e| e.to_string())?;
    Ok("Unpaused".into())
}

// ─── Helpers ────────────────────────────────────────────────────────────────

fn do_transfer(from: &str, to: &str, amount: u64) -> Result<(), String> {
    let fb = storage::get_u64(&balance_key(from)).unwrap_or(0);
    if fb < amount { return Err("Insufficient balance".into()); }
    let tb = storage::get_u64(&balance_key(to)).unwrap_or(0);
    storage::set_u64(&balance_key(from), math::safe_sub(fb, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    storage::set_u64(&balance_key(to), math::safe_add(tb, amount).map_err(|e| e.to_string())?).map_err(|e| e.to_string())?;
    Ok(())
}

fn require_not_paused() -> Result<(), String> {
    if storage::get_bool(KEY_PAUSED) { return Err("Paused".into()); }
    Ok(())
}

fn require_owner() -> Result<(), String> {
    let c = host_functions::get_caller();
    let o = storage::get_string(KEY_OWNER).ok_or("No owner")?;
    if c != o { return Err("Unauthorized".into()); }
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
    fn test_init_and_supply() {
        let r = init("MameyUSD".into(), "MUSD".into(), 6, 1_000_000);
        assert!(r.is_ok());
        assert_eq!(total_supply(), 1_000_000);
        assert_eq!(decimals(), 6);
    }
}
