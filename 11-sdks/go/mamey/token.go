package mamey

import (
	"bytes"
	"encoding/json"
	"fmt"
	"math/big"
	"net/http"
)

type TokenInfo struct {
	Name        string   `json:"name"`
	Symbol      string   `json:"symbol"`
	Decimals    uint8    `json:"decimals"`
	TotalSupply *big.Int `json:"totalSupply"`
	Contract    string   `json:"contract"`
}

type TokenTransfer struct {
	From   string   `json:"from"`
	To     string   `json:"to"`
	Amount *big.Int `json:"amount"`
	Token  string   `json:"token"`
	TxHash string   `json:"txHash"`
}

func (c *Client) GetTokenInfo(contractAddress string) (*TokenInfo, error) {
	resp, err := c.HTTPClient.Get(c.NodeURL + "/api/v1/tokens/" + contractAddress)
	if err != nil {
		return nil, fmt.Errorf("get token info: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("get token info: unexpected status %d", resp.StatusCode)
	}

	var info TokenInfo
	if err := json.NewDecoder(resp.Body).Decode(&info); err != nil {
		return nil, fmt.Errorf("decode token info: %w", err)
	}
	return &info, nil
}

func (c *Client) GetTokenBalance(contractAddress, walletAddress string) (*big.Int, error) {
	url := fmt.Sprintf("%s/api/v1/tokens/%s/balance/%s", c.NodeURL, contractAddress, walletAddress)
	resp, err := c.HTTPClient.Get(url)
	if err != nil {
		return nil, fmt.Errorf("get token balance: %w", err)
	}
	defer resp.Body.Close()

	var result struct {
		Balance *big.Int `json:"balance"`
	}
	if err := json.NewDecoder(resp.Body).Decode(&result); err != nil {
		return nil, fmt.Errorf("decode balance: %w", err)
	}
	return result.Balance, nil
}

func (c *Client) TransferToken(from, to, contractAddress string, amount *big.Int, privateKey string) (*TokenTransfer, error) {
	payload := map[string]interface{}{
		"from":       from,
		"to":         to,
		"amount":     amount.String(),
		"token":      contractAddress,
		"privateKey": privateKey,
	}
	body, _ := json.Marshal(payload)

	resp, err := c.HTTPClient.Post(c.NodeURL+"/api/v1/tokens/transfer", "application/json", bytes.NewReader(body))
	if err != nil {
		return nil, fmt.Errorf("transfer token: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("transfer token: unexpected status %d", resp.StatusCode)
	}

	var transfer TokenTransfer
	if err := json.NewDecoder(resp.Body).Decode(&transfer); err != nil {
		return nil, fmt.Errorf("decode transfer: %w", err)
	}
	return &transfer, nil
}
