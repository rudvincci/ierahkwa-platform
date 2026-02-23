package mamey

import "fmt"

type MameyError struct {
	Code    int    `json:"code"`
	Message string `json:"message"`
	Service string `json:"service"`
}

func (e *MameyError) Error() string {
	return fmt.Sprintf("mamey error %d (%s): %s", e.Code, e.Service, e.Message)
}

var (
	ErrNotFound          = &MameyError{Code: 404, Message: "resource not found", Service: "mameynode"}
	ErrUnauthorized      = &MameyError{Code: 401, Message: "unauthorized", Service: "identity"}
	ErrInsufficientFunds = &MameyError{Code: 400, Message: "insufficient funds", Service: "treasury"}
	ErrInvalidAddress    = &MameyError{Code: 400, Message: "invalid address format", Service: "mameynode"}
	ErrTransactionFailed = &MameyError{Code: 500, Message: "transaction execution failed", Service: "mameynode"}
	ErrConnectionFailed  = &MameyError{Code: 503, Message: "service unavailable", Service: "gateway"}
	ErrRateLimited       = &MameyError{Code: 429, Message: "rate limit exceeded", Service: "gateway"}
)

func IsNotFound(err error) bool {
	if e, ok := err.(*MameyError); ok {
		return e.Code == 404
	}
	return false
}

func IsUnauthorized(err error) bool {
	if e, ok := err.(*MameyError); ok {
		return e.Code == 401
	}
	return false
}
