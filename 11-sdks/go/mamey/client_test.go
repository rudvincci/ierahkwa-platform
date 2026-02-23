package mamey

import (
	"testing"
)

func TestNewClient(t *testing.T) {
	client := NewClient("http://localhost:8545")
	if client == nil {
		t.Fatal("expected non-nil client")
	}
	if client.NodeURL != "http://localhost:8545" {
		t.Errorf("expected NodeURL http://localhost:8545, got %s", client.NodeURL)
	}
	if client.HTTPClient == nil {
		t.Fatal("expected non-nil http client")
	}
}

func TestNewClientWithOptions(t *testing.T) {
	client := NewClient(
		"http://localhost:8545",
	)
	if client.NodeURL != "http://localhost:8545" {
		t.Errorf("unexpected NodeURL: %s", client.NodeURL)
	}
}

func TestMameyError(t *testing.T) {
	err := ErrNotFound
	if err.Error() != "mamey error 404 (mameynode): resource not found" {
		t.Errorf("unexpected error message: %s", err.Error())
	}
}

func TestIsNotFound(t *testing.T) {
	if !IsNotFound(ErrNotFound) {
		t.Error("expected IsNotFound to return true")
	}
	if IsNotFound(ErrUnauthorized) {
		t.Error("expected IsNotFound to return false for unauthorized")
	}
}

func TestIsUnauthorized(t *testing.T) {
	if !IsUnauthorized(ErrUnauthorized) {
		t.Error("expected IsUnauthorized to return true")
	}
	if IsUnauthorized(ErrNotFound) {
		t.Error("expected IsUnauthorized to return false for not found")
	}
}
