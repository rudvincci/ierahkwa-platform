package mamey

import (
	"encoding/json"
	"fmt"
	"math/big"
	"net/http"
)

type Wallet struct {
	Address    string   `json:"address"`
	Balance    *big.Int `json:"balance"`
	Nonce      uint64   `json:"nonce"`
	IsContract bool     `json:"isContract"`
}

type WalletCreate struct {
	Address    string `json:"address"`
	PrivateKey string `json:"privateKey"`
	Mnemonic   string `json:"mnemonic"`
}

func (c *Client) CreateWallet() (*WalletCreate, error) {
	resp, err := c.HTTPClient.Post(c.NodeURL+"/api/v1/wallets", "application/json", nil)
	if err != nil {
		return nil, fmt.Errorf("create wallet: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusCreated {
		return nil, fmt.Errorf("create wallet: unexpected status %d", resp.StatusCode)
	}

	var wallet WalletCreate
	if err := json.NewDecoder(resp.Body).Decode(&wallet); err != nil {
		return nil, fmt.Errorf("decode wallet: %w", err)
	}
	return &wallet, nil
}

func (c *Client) GetWallet(address string) (*Wallet, error) {
	resp, err := c.HTTPClient.Get(c.NodeURL + "/api/v1/wallets/" + address)
	if err != nil {
		return nil, fmt.Errorf("get wallet: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("get wallet %s: unexpected status %d", address, resp.StatusCode)
	}

	var wallet Wallet
	if err := json.NewDecoder(resp.Body).Decode(&wallet); err != nil {
		return nil, fmt.Errorf("decode wallet: %w", err)
	}
	return &wallet, nil
}

func (c *Client) GetWalletBalance(address string) (*big.Int, error) {
	wallet, err := c.GetWallet(address)
	if err != nil {
		return nil, err
	}
	return wallet.Balance, nil
}
