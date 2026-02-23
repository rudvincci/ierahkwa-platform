"""
MameyNode Python SDK
Official SDK for interacting with the Ierahkwa Sovereign Blockchain
Chain ID: 777777
"""

import requests
from typing import Dict, List, Optional, Any
from dataclasses import dataclass
from datetime import datetime

__version__ = "1.0.0"


@dataclass
class ChainInfo:
    chain_id: int
    name: str
    block_height: int
    total_transactions: int
    total_tokens: int
    total_accounts: int


@dataclass
class Token:
    symbol: str
    name: str
    decimals: int
    total_supply: str
    owner: str
    mintable: bool
    burnable: bool


@dataclass
class Identity:
    id: str
    future_wampum_id: str
    citizen_id: str
    email: str
    verification_level: str
    membership_tier: str


class MameySDK:
    """Main SDK client for MameyNode blockchain"""
    
    def __init__(self, node_url: str = "http://localhost:8545"):
        self.node_url = node_url.rstrip('/')
        self.identity_url = "http://localhost:5001"
        self.treasury_url = "http://localhost:5003"
        self.zkp_url = "http://localhost:5002"
        
        self.blockchain = BlockchainModule(self)
        self.identity = IdentityModule(self)
        self.treasury = TreasuryModule(self)
        self.zkp = ZKPModule(self)
    
    def _get(self, url: str) -> Dict:
        response = requests.get(url)
        response.raise_for_status()
        return response.json()
    
    def _post(self, url: str, data: Dict) -> Dict:
        response = requests.post(url, json=data)
        response.raise_for_status()
        return response.json()
    
    def get_chain_info(self) -> ChainInfo:
        data = self._get(f"{self.node_url}/api/v1/stats")
        return ChainInfo(
            chain_id=data.get('chain_id', 777777),
            name="Ierahkwa",
            block_height=data.get('block_height', 0),
            total_transactions=data.get('total_transactions', 0),
            total_tokens=data.get('total_tokens', 0),
            total_accounts=data.get('total_accounts', 0)
        )
    
    def health(self) -> Dict:
        return self._get(f"{self.node_url}/health")


class BlockchainModule:
    """Blockchain operations"""
    
    def __init__(self, sdk: MameySDK):
        self.sdk = sdk
    
    def get_block_number(self) -> int:
        data = self.sdk._get(f"{self.sdk.node_url}/api/v1/stats")
        return data.get('block_height', 1) - 1
    
    def get_block(self, number_or_hash: Any) -> Dict:
        data = self.sdk._get(f"{self.sdk.node_url}/api/v1/blocks/{number_or_hash}")
        return data.get('block', {})
    
    def get_latest_block(self) -> Dict:
        data = self.sdk._get(f"{self.sdk.node_url}/api/v1/blocks/latest")
        return data.get('block', {})
    
    def send_transaction(self, from_addr: str, to_addr: str, value: str, data: Optional[str] = None) -> str:
        result = self.sdk._post(f"{self.sdk.node_url}/api/v1/transactions", {
            "from": from_addr,
            "to": to_addr,
            "value": value,
            "data": data
        })
        return result.get('transaction_hash', '')
    
    def get_balance(self, address: str, token: str = "WAMPUM") -> str:
        data = self.sdk._get(f"{self.sdk.node_url}/api/v1/accounts/{address}/balance")
        return data.get('balances', {}).get(token, '0')
    
    def get_tokens(self) -> List[Token]:
        data = self.sdk._get(f"{self.sdk.node_url}/api/v1/tokens")
        return [Token(**t) for t in data.get('tokens', [])]
    
    def create_token(self, symbol: str, name: str, decimals: int, initial_supply: str, owner: str) -> str:
        result = self.sdk._post(f"{self.sdk.node_url}/api/v1/tokens", {
            "symbol": symbol,
            "name": name,
            "decimals": decimals,
            "initial_supply": initial_supply,
            "owner": owner
        })
        return result.get('symbol', '')


class IdentityModule:
    """Identity operations"""
    
    def __init__(self, sdk: MameySDK):
        self.sdk = sdk
    
    def register(self, first_name: str, last_name: str, email: str, date_of_birth: str, phone: Optional[str] = None) -> Dict:
        result = self.sdk._post(f"{self.sdk.identity_url}/api/v1/identity/register", {
            "firstName": first_name,
            "lastName": last_name,
            "email": email,
            "dateOfBirth": date_of_birth,
            "phone": phone
        })
        return result.get('identity', {})
    
    def get_by_fwid(self, fwid: str) -> Dict:
        data = self.sdk._get(f"{self.sdk.identity_url}/api/v1/identity/fwid/{fwid}")
        return data.get('identity', {})
    
    def verify(self, identity_id: str, level: str, method: str) -> bool:
        result = self.sdk._post(f"{self.sdk.identity_url}/api/v1/identity/{identity_id}/verify", {
            "level": level,
            "method": method
        })
        return result.get('success', False)
    
    def link_wallet(self, identity_id: str, wallet_address: str, public_key: str) -> bool:
        result = self.sdk._post(f"{self.sdk.identity_url}/api/v1/identity/{identity_id}/link-wallet", {
            "walletAddress": wallet_address,
            "publicKey": public_key
        })
        return result.get('success', False)


class TreasuryModule:
    """Treasury operations"""
    
    def __init__(self, sdk: MameySDK):
        self.sdk = sdk
    
    def create_operation(self, op_type: str, amount: float, currency: str, initiated_by: str, **kwargs) -> Dict:
        result = self.sdk._post(f"{self.sdk.treasury_url}/api/v1/treasury/operations", {
            "type": op_type,
            "amount": amount,
            "currency": currency,
            "initiatedBy": initiated_by,
            **kwargs
        })
        return result.get('operation', {})
    
    def get_operation(self, op_id: str) -> Dict:
        data = self.sdk._get(f"{self.sdk.treasury_url}/api/v1/treasury/operations/{op_id}")
        return data.get('operation', {})
    
    def get_accounts(self) -> List[Dict]:
        data = self.sdk._get(f"{self.sdk.treasury_url}/api/v1/treasury/accounts")
        return data.get('accounts', [])


class ZKPModule:
    """Zero Knowledge Proof operations"""
    
    def __init__(self, sdk: MameySDK):
        self.sdk = sdk
    
    def generate_identity_proof(self, fwid: str, prove_age: bool = False) -> Dict:
        result = self.sdk._post(f"{self.sdk.zkp_url}/api/v1/zkp/identity", {
            "futureWampumId": fwid,
            "proveAge": prove_age
        })
        return result.get('proof', {})
    
    def generate_balance_proof(self, address: str, token: str, min_balance: str) -> Dict:
        result = self.sdk._post(f"{self.sdk.zkp_url}/api/v1/zkp/balance", {
            "address": address,
            "token": token,
            "minimumBalance": min_balance
        })
        return result.get('proof', {})
    
    def verify_proof(self, proof_id: str, verifier_address: str) -> bool:
        result = self.sdk._post(f"{self.sdk.zkp_url}/api/v1/zkp/verify/{proof_id}", {
            "verifierAddress": verifier_address
        })
        return result.get('valid', False)


# Convenience function
def create_client(node_url: str = "http://localhost:8545") -> MameySDK:
    """Create a new MameySDK client"""
    return MameySDK(node_url)
