/**
 * MameyNode TypeScript SDK
 * Official SDK for interacting with the Ierahkwa Sovereign Blockchain
 * Chain ID: 777777
 */

import axios, { AxiosInstance } from 'axios';

// Types
export interface ChainInfo {
  chainId: number;
  name: string;
  blockHeight: number;
  totalTransactions: number;
  totalTokens: number;
  totalAccounts: number;
}

export interface Block {
  number: number;
  hash: string;
  parentHash: string;
  timestamp: Date;
  miner: string;
  gasUsed: number;
  gasLimit: number;
  transactions: string[];
}

export interface Transaction {
  hash: string;
  from: string;
  to: string;
  value: string;
  data?: string;
  nonce: number;
  gasPrice: number;
  gasLimit: number;
  status: string;
}

export interface Account {
  address: string;
  balances: Record<string, string>;
  nonce: number;
  futureWampumId?: string;
}

export interface Token {
  symbol: string;
  name: string;
  decimals: number;
  totalSupply: string;
  owner: string;
  mintable: boolean;
  burnable: boolean;
}

export interface Identity {
  id: string;
  futureWampumId: string;
  citizenId: string;
  email: string;
  verificationLevel: string;
  membershipTier: string;
}

export interface ZKProof {
  id: string;
  proofId: string;
  type: string;
  proofData: string;
  isVerified: boolean;
  statement?: string;
}

// Main SDK Class
export class MameySDK {
  private client: AxiosInstance;
  private nodeUrl: string;
  
  public blockchain: BlockchainModule;
  public identity: IdentityModule;
  public treasury: TreasuryModule;
  public zkp: ZKPModule;
  
  constructor(nodeUrl: string = 'http://localhost:8545') {
    this.nodeUrl = nodeUrl;
    this.client = axios.create({
      baseURL: nodeUrl,
      headers: { 'Content-Type': 'application/json' }
    });
    
    this.blockchain = new BlockchainModule(this.client);
    this.identity = new IdentityModule(this.client);
    this.treasury = new TreasuryModule(this.client);
    this.zkp = new ZKPModule(this.client);
  }
  
  async getChainInfo(): Promise<ChainInfo> {
    const response = await this.client.get('/api/v1/stats');
    return response.data;
  }
  
  async health(): Promise<{ status: string }> {
    const response = await this.client.get('/health');
    return response.data;
  }
}

// Blockchain Module
class BlockchainModule {
  constructor(private client: AxiosInstance) {}
  
  async getBlockNumber(): Promise<number> {
    const response = await this.client.get('/api/v1/stats');
    return response.data.block_height - 1;
  }
  
  async getBlock(numberOrHash: number | string): Promise<Block> {
    const response = await this.client.get(`/api/v1/blocks/${numberOrHash}`);
    return response.data.block;
  }
  
  async getLatestBlock(): Promise<Block> {
    const response = await this.client.get('/api/v1/blocks/latest');
    return response.data.block;
  }
  
  async sendTransaction(tx: {
    from: string;
    to: string;
    value: string;
    data?: string;
  }): Promise<string> {
    const response = await this.client.post('/api/v1/transactions', tx);
    return response.data.transaction_hash;
  }
  
  async getTransaction(hash: string): Promise<Transaction> {
    const response = await this.client.get(`/api/v1/transactions/${hash}`);
    return response.data.transaction;
  }
  
  async getAccount(address: string): Promise<Account> {
    const response = await this.client.get(`/api/v1/accounts/${address}`);
    return response.data.account;
  }
  
  async getBalance(address: string, token: string = 'WAMPUM'): Promise<string> {
    const response = await this.client.get(`/api/v1/accounts/${address}/balance`);
    return response.data.balances[token] || '0';
  }
  
  async getTokens(): Promise<Token[]> {
    const response = await this.client.get('/api/v1/tokens');
    return response.data.tokens;
  }
  
  async createToken(params: {
    symbol: string;
    name: string;
    decimals: number;
    initialSupply: string;
    owner: string;
  }): Promise<string> {
    const response = await this.client.post('/api/v1/tokens', params);
    return response.data.symbol;
  }
}

// Identity Module
class IdentityModule {
  private baseUrl = 'http://localhost:5001'; // Identity service
  
  constructor(private client: AxiosInstance) {}
  
  async register(params: {
    firstName: string;
    lastName: string;
    email: string;
    dateOfBirth: Date;
    phone?: string;
  }): Promise<Identity> {
    const response = await axios.post(`${this.baseUrl}/api/v1/identity/register`, params);
    return response.data.identity;
  }
  
  async getByFwId(fwid: string): Promise<Identity> {
    const response = await axios.get(`${this.baseUrl}/api/v1/identity/fwid/${fwid}`);
    return response.data.identity;
  }
  
  async verify(id: string, params: {
    level: string;
    method: string;
    biometricHash?: string;
    kycProofHash?: string;
  }): Promise<boolean> {
    const response = await axios.post(`${this.baseUrl}/api/v1/identity/${id}/verify`, params);
    return response.data.success;
  }
  
  async linkWallet(id: string, walletAddress: string, publicKey: string): Promise<boolean> {
    const response = await axios.post(`${this.baseUrl}/api/v1/identity/${id}/link-wallet`, {
      walletAddress,
      publicKey
    });
    return response.data.success;
  }
  
  async setMembership(id: string, tier: 'Bronze' | 'Silver' | 'Gold' | 'Platinum'): Promise<boolean> {
    const tierMap = { Bronze: 1, Silver: 2, Gold: 3, Platinum: 4 };
    const response = await axios.post(`${this.baseUrl}/api/v1/identity/${id}/membership`, {
      tier: tierMap[tier]
    });
    return response.data.success;
  }
}

// Treasury Module
class TreasuryModule {
  private baseUrl = 'http://localhost:5002'; // Treasury service
  
  constructor(private client: AxiosInstance) {}
  
  async createOperation(params: {
    type: string;
    amount: number;
    currency: string;
    initiatedBy: string;
    toAccount?: string;
    description?: string;
  }): Promise<any> {
    const response = await axios.post(`${this.baseUrl}/api/v1/treasury/operations`, params);
    return response.data.operation;
  }
  
  async getOperation(id: string): Promise<any> {
    const response = await axios.get(`${this.baseUrl}/api/v1/treasury/operations/${id}`);
    return response.data.operation;
  }
  
  async approveOperation(id: string, params: {
    approverId: string;
    approverName: string;
    approverRole: string;
    decision: 'Approved' | 'Rejected';
    comments?: string;
  }): Promise<boolean> {
    const response = await axios.post(`${this.baseUrl}/api/v1/treasury/operations/${id}/approve`, params);
    return response.data.success;
  }
  
  async getAccounts(): Promise<any[]> {
    const response = await axios.get(`${this.baseUrl}/api/v1/treasury/accounts`);
    return response.data.accounts;
  }
}

// ZKP Module
class ZKPModule {
  private baseUrl = 'http://localhost:5003'; // ZKP service
  
  constructor(private client: AxiosInstance) {}
  
  async generateIdentityProof(fwid: string, proveAge: boolean = false): Promise<ZKProof> {
    const response = await axios.post(`${this.baseUrl}/api/v1/zkp/identity`, {
      futureWampumId: fwid,
      proveAge
    });
    return response.data.proof;
  }
  
  async generateBalanceProof(address: string, token: string, minBalance: string): Promise<ZKProof> {
    const response = await axios.post(`${this.baseUrl}/api/v1/zkp/balance`, {
      address,
      token,
      minimumBalance: minBalance
    });
    return response.data.proof;
  }
  
  async generateMembershipProof(fwid: string, minimumTier: number): Promise<ZKProof> {
    const response = await axios.post(`${this.baseUrl}/api/v1/zkp/membership`, {
      futureWampumId: fwid,
      minimumTier
    });
    return response.data.proof;
  }
  
  async verifyProof(proofId: string, verifierAddress: string): Promise<boolean> {
    const response = await axios.post(`${this.baseUrl}/api/v1/zkp/verify/${proofId}`, {
      verifierAddress
    });
    return response.data.valid;
  }
}

// Export default instance
export default MameySDK;

// Usage example:
/*
import MameySDK from '@mamey-io/mamey-sdk';

const mamey = new MameySDK('http://localhost:8545');

// Get chain info
const info = await mamey.getChainInfo();
console.log(`Chain ID: ${info.chainId}, Blocks: ${info.blockHeight}`);

// Register identity
const identity = await mamey.identity.register({
  firstName: 'John',
  lastName: 'Doe',
  email: 'john@example.com',
  dateOfBirth: new Date('1990-01-01')
});
console.log(`FutureWampumID: ${identity.futureWampumId}`);

// Generate ZK proof
const proof = await mamey.zkp.generateIdentityProof(identity.futureWampumId);
console.log(`Proof generated: ${proof.proofId}`);
*/
