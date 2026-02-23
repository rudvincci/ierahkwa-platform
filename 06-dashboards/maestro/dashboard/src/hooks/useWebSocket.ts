import { useEffect, useRef, useState, useCallback } from 'react';
import type { DashboardMessage } from '../types/websocket';

interface UseWebSocketOptions {
  url: string;
  onMessage?: (message: DashboardMessage) => void;
  onError?: (error: Event) => void;
  onOpen?: () => void;
  onClose?: () => void;
  reconnect?: boolean;
  reconnectInterval?: number;
  maxReconnectInterval?: number;
}

export function useWebSocket({
  url,
  onMessage,
  onError,
  onOpen,
  onClose,
  reconnect = true,
  reconnectInterval = 1000,
  maxReconnectInterval = 30000,
}: UseWebSocketOptions) {
  const [isConnected, setIsConnected] = useState(false);
  const [reconnectAttempts, setReconnectAttempts] = useState(0);
  const wsRef = useRef<WebSocket | null>(null);
  const reconnectTimeoutRef = useRef<number | null>(null);
  const shouldReconnectRef = useRef(true);

  const connect = useCallback(() => {
    if (!shouldReconnectRef.current) return;

    // Don't try to connect if URL is invalid
    if (!url || url === '') {
      console.warn('WebSocket URL is empty, skipping connection');
      return;
    }

    // Don't connect if already connecting or connected
    if (wsRef.current) {
      const state = wsRef.current.readyState;
      if (state === WebSocket.CONNECTING || state === WebSocket.OPEN) {
        console.log('WebSocket already connecting/connected, skipping');
        return;
      }
    }

    try {
      console.log('Creating WebSocket connection to:', url);
      const ws = new WebSocket(url);
      wsRef.current = ws;

      ws.onopen = () => {
        console.log('WebSocket connected:', url);
        setIsConnected(true);
        setReconnectAttempts(0);
        onOpen?.();
      };

      ws.onmessage = (event) => {
        try {
          const message: DashboardMessage = JSON.parse(event.data);
          // Convert timestamp string to Date if needed
          if (typeof message.timestamp === 'string') {
            message.timestamp = new Date(message.timestamp);
          }
          onMessage?.(message);
        } catch (error) {
          console.error('Failed to parse WebSocket message:', error);
        }
      };

      ws.onerror = (error) => {
        console.error('WebSocket error:', error);
        onError?.(error);
      };

      ws.onclose = (event) => {
        console.log('WebSocket closed:', event.code, event.reason);
        setIsConnected(false);
        onClose?.();

        // Clear the ref if we're not reconnecting
        if (!reconnect || !shouldReconnectRef.current || event.code === 1000) {
          wsRef.current = null;
          return;
        }

        // Only reconnect if it wasn't a clean close and we should reconnect
        const delay = Math.min(
          reconnectInterval * Math.pow(2, reconnectAttempts),
          maxReconnectInterval
        );
        console.log(`WebSocket reconnecting in ${delay}ms (attempt ${reconnectAttempts + 1})...`);
        reconnectTimeoutRef.current = window.setTimeout(() => {
          if (shouldReconnectRef.current) {
            setReconnectAttempts((prev) => prev + 1);
            connect();
          }
        }, delay);
      };
    } catch (error) {
      console.error('WebSocket connection error:', error);
      wsRef.current = null;
      
      if (reconnect && shouldReconnectRef.current) {
        const delay = Math.min(
          reconnectInterval * Math.pow(2, reconnectAttempts),
          maxReconnectInterval
        );
        reconnectTimeoutRef.current = window.setTimeout(() => {
          if (shouldReconnectRef.current) {
            setReconnectAttempts((prev) => prev + 1);
            connect();
          }
        }, delay);
      }
    }
  }, [url, reconnect, reconnectInterval, maxReconnectInterval]);

  // Store callbacks in refs to avoid recreating connect function
  const onMessageRef = useRef(onMessage);
  const onErrorRef = useRef(onError);
  const onOpenRef = useRef(onOpen);
  const onCloseRef = useRef(onClose);

  // Update refs when callbacks change
  useEffect(() => {
    onMessageRef.current = onMessage;
    onErrorRef.current = onError;
    onOpenRef.current = onOpen;
    onCloseRef.current = onClose;
  }, [onMessage, onError, onOpen, onClose]);

  // Create a stable connect function that uses refs
  const stableConnect = useCallback(() => {
    if (!shouldReconnectRef.current) return;
    if (!url || url === '') {
      console.warn('WebSocket URL is empty, skipping connection');
      return;
    }
    if (wsRef.current) {
      const state = wsRef.current.readyState;
      if (state === WebSocket.CONNECTING || state === WebSocket.OPEN) {
        return;
      }
    }

    try {
      console.log('Creating WebSocket connection to:', url);
      const ws = new WebSocket(url);
      wsRef.current = ws;

      ws.onopen = () => {
        console.log('WebSocket connected:', url);
        setIsConnected(true);
        setReconnectAttempts(0);
        onOpenRef.current?.();
      };

      ws.onmessage = (event) => {
        try {
          const message: DashboardMessage = JSON.parse(event.data);
          if (typeof message.timestamp === 'string') {
            message.timestamp = new Date(message.timestamp);
          }
          onMessageRef.current?.(message);
        } catch (error) {
          console.error('Failed to parse WebSocket message:', error);
        }
      };

      ws.onerror = (error) => {
        console.error('WebSocket error:', error);
        onErrorRef.current?.(error);
      };

      ws.onclose = (event) => {
        console.log('WebSocket closed:', event.code, event.reason);
        setIsConnected(false);
        onCloseRef.current?.();

        if (!reconnect || !shouldReconnectRef.current || event.code === 1000) {
          wsRef.current = null;
          return;
        }

        const delay = Math.min(
          reconnectInterval * Math.pow(2, reconnectAttempts),
          maxReconnectInterval
        );
        console.log(`WebSocket reconnecting in ${delay}ms (attempt ${reconnectAttempts + 1})...`);
        reconnectTimeoutRef.current = window.setTimeout(() => {
          if (shouldReconnectRef.current) {
            setReconnectAttempts((prev) => prev + 1);
            stableConnect();
          }
        }, delay);
      };
    } catch (error) {
      console.error('WebSocket connection error:', error);
      wsRef.current = null;
      
      if (reconnect && shouldReconnectRef.current) {
        const delay = Math.min(
          reconnectInterval * Math.pow(2, reconnectAttempts),
          maxReconnectInterval
        );
        reconnectTimeoutRef.current = window.setTimeout(() => {
          if (shouldReconnectRef.current) {
            setReconnectAttempts((prev) => prev + 1);
            stableConnect();
          }
        }, delay);
      }
    }
  }, [url, reconnect, reconnectInterval, maxReconnectInterval, reconnectAttempts]);

  useEffect(() => {
    shouldReconnectRef.current = true;
    
    // Small delay to avoid rapid connect/disconnect cycles
    const connectTimeout = window.setTimeout(() => {
      if (shouldReconnectRef.current) {
        stableConnect();
      }
    }, 200);

    return () => {
      shouldReconnectRef.current = false;
      
      if (connectTimeout) {
        clearTimeout(connectTimeout);
      }
      
      if (reconnectTimeoutRef.current !== null) {
        clearTimeout(reconnectTimeoutRef.current);
        reconnectTimeoutRef.current = null;
      }
      
      if (wsRef.current) {
        const state = wsRef.current.readyState;
        if (state === WebSocket.CONNECTING || state === WebSocket.OPEN) {
          wsRef.current.close(1000, 'Component unmounting');
        }
        wsRef.current = null;
      }
    };
  }, [stableConnect]);

  const send = useCallback((data: any) => {
    if (wsRef.current?.readyState === WebSocket.OPEN) {
      wsRef.current.send(JSON.stringify(data));
    }
  }, []);

  return { isConnected, send };
}
