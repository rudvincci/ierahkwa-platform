// Package mamey provides a Go SDK for MameyNode blockchain
package mamey

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"
)

// Client is the main MameyNode SDK client
type Client struct {
	NodeURL     string
	IdentityURL string
	TreasuryURL string
	ZKPURL      string
	HTTPClient  *http.Client
}

// NewClient creates a new MameyNode client
func NewClient(nodeURL string) *Client {
	return &Client{
		NodeURL:     nodeURL,
		IdentityURL: "http://localhost:5001",
		TreasuryURL: "http://localhost:5003",
		ZKPURL:      "http://localhost:5002",
		HTTPClient: &http.Client{
			Timeout: 30 * time.Second,
		},
	}
}

// ChainInfo represents blockchain information
type ChainInfo struct {
	ChainID           uint64 `json:"chain_id"`
	Name              string `json:"name"`
	BlockHeight       uint64 `json:"block_height"`
	TotalTransactions uint64 `json:"total_transactions"`
	TotalTokens       int    `json:"total_tokens"`
	TotalAccounts     int    `json:"total_accounts"`
}

// Token represents a token on the blockchain
type Token struct {
	Symbol      string `json:"symbol"`
	Name        string `json:"name"`
	Decimals    int    `json:"decimals"`
	TotalSupply string `json:"total_supply"`
	Owner       string `json:"owner"`
	Mintable    bool   `json:"mintable"`
	Burnable    bool   `json:"burnable"`
}

// Block represents a blockchain block
type Block struct {
	Number       uint64   `json:"number"`
	Hash         string   `json:"hash"`
	ParentHash   string   `json:"parent_hash"`
	Timestamp    int64    `json:"timestamp"`
	Miner        string   `json:"miner"`
	GasUsed      uint64   `json:"gas_used"`
	Transactions []string `json:"transactions"`
}

// Transaction represents a blockchain transaction
type Transaction struct {
	Hash     string `json:"hash"`
	From     string `json:"from"`
	To       string `json:"to"`
	Value    string `json:"value"`
	Data     string `json:"data,omitempty"`
	Nonce    uint64 `json:"nonce"`
	GasPrice uint64 `json:"gas_price"`
	Status   string `json:"status"`
}

// GetChainInfo returns blockchain information
func (c *Client) GetChainInfo() (*ChainInfo, error) {
	resp, err := c.get(c.NodeURL + "/api/v1/stats")
	if err != nil {
		return nil, err
	}
	
	var info ChainInfo
	if err := json.Unmarshal(resp, &info); err != nil {
		return nil, err
	}
	info.Name = "Ierahkwa"
	return &info, nil
}

// GetBlockNumber returns the latest block number
func (c *Client) GetBlockNumber() (uint64, error) {
	info, err := c.GetChainInfo()
	if err != nil {
		return 0, err
	}
	return info.BlockHeight - 1, nil
}

// GetBlock returns a block by number or hash
func (c *Client) GetBlock(numberOrHash interface{}) (*Block, error) {
	url := fmt.Sprintf("%s/api/v1/blocks/%v", c.NodeURL, numberOrHash)
	resp, err := c.get(url)
	if err != nil {
		return nil, err
	}
	
	var result struct {
		Block Block `json:"block"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, err
	}
	return &result.Block, nil
}

// GetLatestBlock returns the latest block
func (c *Client) GetLatestBlock() (*Block, error) {
	resp, err := c.get(c.NodeURL + "/api/v1/blocks/latest")
	if err != nil {
		return nil, err
	}
	
	var result struct {
		Block Block `json:"block"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, err
	}
	return &result.Block, nil
}

// SendTransaction sends a transaction
func (c *Client) SendTransaction(from, to, value string, data *string) (string, error) {
	payload := map[string]interface{}{
		"from":  from,
		"to":    to,
		"value": value,
	}
	if data != nil {
		payload["data"] = *data
	}
	
	resp, err := c.post(c.NodeURL+"/api/v1/transactions", payload)
	if err != nil {
		return "", err
	}
	
	var result struct {
		TxHash string `json:"transaction_hash"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return "", err
	}
	return result.TxHash, nil
}

// GetBalance returns account balance for a token
func (c *Client) GetBalance(address, token string) (string, error) {
	url := fmt.Sprintf("%s/api/v1/accounts/%s/balance", c.NodeURL, address)
	resp, err := c.get(url)
	if err != nil {
		return "0", err
	}
	
	var result struct {
		Balances map[string]string `json:"balances"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return "0", err
	}
	
	if balance, ok := result.Balances[token]; ok {
		return balance, nil
	}
	return "0", nil
}

// GetTokens returns all tokens
func (c *Client) GetTokens() ([]Token, error) {
	resp, err := c.get(c.NodeURL + "/api/v1/tokens")
	if err != nil {
		return nil, err
	}
	
	var result struct {
		Tokens []Token `json:"tokens"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return nil, err
	}
	return result.Tokens, nil
}

// CreateToken creates a new token
func (c *Client) CreateToken(symbol, name string, decimals int, initialSupply, owner string) (string, error) {
	payload := map[string]interface{}{
		"symbol":         symbol,
		"name":           name,
		"decimals":       decimals,
		"initial_supply": initialSupply,
		"owner":          owner,
	}
	
	resp, err := c.post(c.NodeURL+"/api/v1/tokens", payload)
	if err != nil {
		return "", err
	}
	
	var result struct {
		Symbol string `json:"symbol"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return "", err
	}
	return result.Symbol, nil
}

// Health checks node health
func (c *Client) Health() (bool, error) {
	resp, err := c.get(c.NodeURL + "/health")
	if err != nil {
		return false, err
	}
	
	var result struct {
		Status string `json:"status"`
	}
	if err := json.Unmarshal(resp, &result); err != nil {
		return false, err
	}
	return result.Status == "healthy", nil
}

// HTTP helpers
func (c *Client) get(url string) ([]byte, error) {
	resp, err := c.HTTPClient.Get(url)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()
	return io.ReadAll(resp.Body)
}

func (c *Client) post(url string, data interface{}) ([]byte, error) {
	body, err := json.Marshal(data)
	if err != nil {
		return nil, err
	}
	
	resp, err := c.HTTPClient.Post(url, "application/json", bytes.NewBuffer(body))
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()
	return io.ReadAll(resp.Body)
}
