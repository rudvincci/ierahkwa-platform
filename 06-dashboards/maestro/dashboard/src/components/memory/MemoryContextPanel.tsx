/**
 * Memory Context Panel
 * Displays smart context loading functionality
 */

import { useState, useCallback } from 'react';
import './MemoryContextPanel.css';

interface LoadedContext {
  content: string;
  files_loaded: number;
  total_size: number;
  max_size: number;
  trigger: string;
  mode: string;
  timestamp: Date | string;
}

export default function MemoryContextPanel() {
  const [trigger, setTrigger] = useState('conversation_start');
  const [context, setContext] = useState<LoadedContext | null>(null);
  const [loading, setLoading] = useState(false);

  const handleLoadContext = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch(`/api/memory/context?trigger=${encodeURIComponent(trigger)}`);
      const data = await response.json();
      setContext(data);
    } catch (error) {
      console.error('Failed to load context:', error);
    } finally {
      setLoading(false);
    }
  }, [trigger]);

  return (
    <div className="memory-context-panel">
      <h2>Smart Context Loading</h2>
      
      <div className="context-controls">
        <label>
          Trigger:
          <select
            value={trigger}
            onChange={(e) => setTrigger(e.target.value)}
          >
            <option value="conversation_start">Conversation Start</option>
            <option value="keyword:">Keyword (specify below)</option>
            <option value="file_pattern:">File Pattern (specify below)</option>
          </select>
        </label>
        {(trigger.startsWith('keyword:') || trigger.startsWith('file_pattern:')) && (
          <input
            type="text"
            placeholder={trigger.startsWith('keyword:') ? 'Enter keywords...' : 'Enter file pattern...'}
            onChange={(e) => setTrigger(`${trigger.split(':')[0]}:${e.target.value}`)}
          />
        )}
        <button
          className="btn btn-primary"
          onClick={handleLoadContext}
          disabled={loading}
        >
          {loading ? 'Loading...' : 'Load Context'}
        </button>
      </div>

      {context && (
        <div className="context-display">
          <div className="context-meta">
            <span>Files: {context.files_loaded}</span>
            <span>Size: {Math.floor(context.total_size / 1024)}KB / {Math.floor(context.max_size / 1024)}KB</span>
            <span>Mode: {context.mode}</span>
            <span>Trigger: {context.trigger}</span>
          </div>
          <div className="context-content">
            <pre>{context.content}</pre>
          </div>
        </div>
      )}
    </div>
  );
}

