package mamey

import (
	"bytes"
	"encoding/json"
	"fmt"
	"math/big"
	"net/http"
)

type DetailedTransaction struct {
	Hash      string   `json:"hash"`
	From      string   `json:"from"`
	To        string   `json:"to"`
	Value     *big.Int `json:"value"`
	Gas       uint64   `json:"gas"`
	GasPrice  *big.Int `json:"gasPrice"`
	Nonce     uint64   `json:"nonce"`
	Status    string   `json:"status"`
	BlockNum  uint64   `json:"blockNumber"`
	Timestamp int64    `json:"timestamp"`
}

type TransactionRequest struct {
	From       string   `json:"from"`
	To         string   `json:"to"`
	Value      *big.Int `json:"value"`
	Data       string   `json:"data,omitempty"`
	PrivateKey string   `json:"privateKey"`
}

type TransactionReceipt struct {
	TxHash   string `json:"transactionHash"`
	Status   string `json:"status"`
	BlockNum uint64 `json:"blockNumber"`
	GasUsed  uint64 `json:"gasUsed"`
	Logs     []Log  `json:"logs"`
}

type Log struct {
	Address string   `json:"address"`
	Topics  []string `json:"topics"`
	Data    string   `json:"data"`
}

func (c *Client) SendDetailedTransaction(req *TransactionRequest) (*TransactionReceipt, error) {
	body, err := json.Marshal(req)
	if err != nil {
		return nil, fmt.Errorf("marshal request: %w", err)
	}

	resp, err := c.HTTPClient.Post(c.NodeURL+"/api/v1/transactions", "application/json", bytes.NewReader(body))
	if err != nil {
		return nil, fmt.Errorf("send transaction: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated {
		return nil, fmt.Errorf("send transaction: unexpected status %d", resp.StatusCode)
	}

	var receipt TransactionReceipt
	if err := json.NewDecoder(resp.Body).Decode(&receipt); err != nil {
		return nil, fmt.Errorf("decode receipt: %w", err)
	}
	return &receipt, nil
}

func (c *Client) GetDetailedTransaction(txHash string) (*DetailedTransaction, error) {
	resp, err := c.HTTPClient.Get(c.NodeURL + "/api/v1/transactions/" + txHash)
	if err != nil {
		return nil, fmt.Errorf("get transaction: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("get transaction: unexpected status %d", resp.StatusCode)
	}

	var tx DetailedTransaction
	if err := json.NewDecoder(resp.Body).Decode(&tx); err != nil {
		return nil, fmt.Errorf("decode transaction: %w", err)
	}
	return &tx, nil
}

func (c *Client) GetTransactionReceipt(txHash string) (*TransactionReceipt, error) {
	resp, err := c.HTTPClient.Get(c.NodeURL + "/api/v1/transactions/" + txHash + "/receipt")
	if err != nil {
		return nil, fmt.Errorf("get receipt: %w", err)
	}
	defer resp.Body.Close()

	var receipt TransactionReceipt
	if err := json.NewDecoder(resp.Body).Decode(&receipt); err != nil {
		return nil, fmt.Errorf("decode receipt: %w", err)
	}
	return &receipt, nil
}

func (c *Client) GetPendingTransactions() ([]DetailedTransaction, error) {
	resp, err := c.HTTPClient.Get(c.NodeURL + "/api/v1/transactions/pending")
	if err != nil {
		return nil, fmt.Errorf("get pending: %w", err)
	}
	defer resp.Body.Close()

	var txs []DetailedTransaction
	if err := json.NewDecoder(resp.Body).Decode(&txs); err != nil {
		return nil, fmt.Errorf("decode pending: %w", err)
	}
	return txs, nil
}
