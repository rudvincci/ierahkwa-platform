//! P2P networking for MameyNode

use std::collections::HashMap;
use tokio::sync::mpsc;
use serde::{Deserialize, Serialize};

/// Network message types
#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum NetworkMessage {
    // Block propagation
    NewBlock(BlockAnnouncement),
    GetBlock(String),
    Block(BlockData),
    
    // Transaction propagation
    NewTransaction(TransactionAnnouncement),
    GetTransaction(String),
    Transaction(TransactionData),
    
    // Peer discovery
    Ping(u64),
    Pong(u64),
    GetPeers,
    Peers(Vec<PeerInfo>),
    
    // State sync
    GetState(String),
    State(StateData),
    
    // Consensus
    ValidatorVote(Vote),
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct BlockAnnouncement {
    pub number: u64,
    pub hash: String,
    pub parent_hash: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct BlockData {
    pub number: u64,
    pub hash: String,
    pub data: Vec<u8>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TransactionAnnouncement {
    pub hash: String,
    pub from: String,
    pub to: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TransactionData {
    pub hash: String,
    pub data: Vec<u8>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct PeerInfo {
    pub peer_id: String,
    pub address: String,
    pub chain_id: u64,
    pub block_height: u64,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct StateData {
    pub root: String,
    pub data: Vec<u8>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Vote {
    pub block_hash: String,
    pub validator: String,
    pub signature: String,
}

/// Peer connection state
#[derive(Debug, Clone)]
pub struct Peer {
    pub id: String,
    pub address: String,
    pub chain_id: u64,
    pub block_height: u64,
    pub connected_at: i64,
    pub last_seen: i64,
}

/// Network manager
pub struct NetworkManager {
    peers: HashMap<String, Peer>,
    local_peer_id: String,
    chain_id: u64,
    max_peers: usize,
}

impl NetworkManager {
    pub fn new(chain_id: u64, max_peers: usize) -> Self {
        Self {
            peers: HashMap::new(),
            local_peer_id: uuid::Uuid::new_v4().to_string(),
            chain_id,
            max_peers,
        }
    }
    
    pub fn add_peer(&mut self, peer: Peer) -> bool {
        if self.peers.len() >= self.max_peers {
            return false;
        }
        
        if peer.chain_id != self.chain_id {
            return false;
        }
        
        self.peers.insert(peer.id.clone(), peer);
        true
    }
    
    pub fn remove_peer(&mut self, peer_id: &str) -> Option<Peer> {
        self.peers.remove(peer_id)
    }
    
    pub fn get_peer(&self, peer_id: &str) -> Option<&Peer> {
        self.peers.get(peer_id)
    }
    
    pub fn get_peers(&self) -> Vec<&Peer> {
        self.peers.values().collect()
    }
    
    pub fn peer_count(&self) -> usize {
        self.peers.len()
    }
    
    pub fn broadcast_block(&self, announcement: BlockAnnouncement) {
        // In production, this would send to all peers
        tracing::debug!("Broadcasting block {}", announcement.hash);
    }
    
    pub fn broadcast_transaction(&self, announcement: TransactionAnnouncement) {
        tracing::debug!("Broadcasting transaction {}", announcement.hash);
    }
}
