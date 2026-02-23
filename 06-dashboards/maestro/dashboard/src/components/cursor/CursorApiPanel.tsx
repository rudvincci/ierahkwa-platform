/**
 * Cursor API Panel
 * Displays API key configuration and status
 */

import { useState, useEffect, useCallback } from 'react';
import './CursorApiPanel.css';

export default function CursorApiPanel() {
  const [apiKey, setApiKey] = useState('');
  const [maskedKey, setMaskedKey] = useState<string | null>(null);
  const [isConfigured, setIsConfigured] = useState(false);
  const [health, setHealth] = useState<{ status: string; message?: string } | null>(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  const loadStatus = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch('/api/cursor/api-key');
      const data = await response.json();
      setIsConfigured(data.configured || false);
      setMaskedKey(data.api_key || null);
    } catch (error) {
      console.error('Failed to load API key status:', error);
    } finally {
      setLoading(false);
    }
  }, []);

  const checkHealth = useCallback(async () => {
    try {
      const response = await fetch('/api/cursor/health');
      const data = await response.json();
      setHealth(data);
    } catch (error) {
      console.error('Failed to check health:', error);
      setHealth({ status: 'error', message: 'Health check failed' });
    }
  }, []);

  const handleSaveApiKey = useCallback(async () => {
    if (!apiKey.trim()) return;

    setSaving(true);
    try {
      const response = await fetch('/api/cursor/api-key', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ api_key: apiKey }),
      });

      if (response.ok) {
        alert('API key saved successfully!');
        setApiKey('');
        loadStatus();
        checkHealth();
      } else {
        const error = await response.json();
        alert(`Failed to save API key: ${error.error}`);
      }
    } catch (error) {
      console.error('Failed to save API key:', error);
      alert('Failed to save API key');
    } finally {
      setSaving(false);
    }
  }, [apiKey, loadStatus, checkHealth]);

  useEffect(() => {
    loadStatus();
    checkHealth();
  }, [loadStatus, checkHealth]);

  return (
    <div className="cursor-api-panel">
      <h2>Cursor API Configuration</h2>

      <div className="api-key-section">
        <h3>API Key</h3>
        {isConfigured && maskedKey ? (
          <div className="api-key-status">
            <p>API Key: <code>{maskedKey}</code></p>
            <p className="status-text">Status: Configured</p>
          </div>
        ) : (
          <div className="api-key-input">
            <input
              type="password"
              placeholder="Enter Cursor API key..."
              value={apiKey}
              onChange={(e) => setApiKey(e.target.value)}
            />
            <button
              className="btn btn-primary"
              onClick={handleSaveApiKey}
              disabled={saving || !apiKey.trim()}
            >
              {saving ? 'Saving...' : 'Save API Key'}
            </button>
          </div>
        )}
      </div>

      <div className="health-section">
        <h3>Health Status</h3>
        {health && (
          <div className={`health-status ${health.status}`}>
            <span className="status-indicator"></span>
            <span className="status-text">
              {health.status === 'ok' ? 'Connected' : 'Not Available'}
            </span>
            {health.message && (
              <span className="status-message">{health.message}</span>
            )}
          </div>
        )}
        <button
          className="btn btn-secondary"
          onClick={checkHealth}
          disabled={loading}
        >
          {loading ? 'Checking...' : 'Check Health'}
        </button>
      </div>
    </div>
  );
}

