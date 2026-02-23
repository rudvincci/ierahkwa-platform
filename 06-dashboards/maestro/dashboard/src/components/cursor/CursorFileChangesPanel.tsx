/**
 * Cursor File Changes Panel
 * Displays file change tracking
 */

import { useState, useEffect, useCallback } from 'react';
import './CursorFileChangesPanel.css';

interface FileChange {
  path: string;
  type: 'created' | 'modified' | 'deleted';
  timestamp: Date | string;
  diff?: string;
}

interface ChangesSummary {
  totalChanges: number;
  filesChanged: number;
  additions: number;
  deletions: number;
  byType: Record<string, number>;
}

export default function CursorFileChangesPanel() {
  const [changes, setChanges] = useState<FileChange[]>([]);
  const [summary, setSummary] = useState<ChangesSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [days, setDays] = useState(7);
  const [limit, setLimit] = useState(50);

  const loadChanges = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch(`/api/cursor/files?limit=${limit}`);
      const data = await response.json();
      setChanges(data.changes || []);
    } catch (error) {
      console.error('Failed to load file changes:', error);
    } finally {
      setLoading(false);
    }
  }, [limit]);

  const loadSummary = useCallback(async () => {
    try {
      const response = await fetch(`/api/cursor/files/summary?days=${days}`);
      const data = await response.json();
      setSummary(data.summary);
    } catch (error) {
      console.error('Failed to load summary:', error);
    }
  }, [days]);

  useEffect(() => {
    loadChanges();
    loadSummary();
    const interval = setInterval(() => {
      loadChanges();
      loadSummary();
    }, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, [loadChanges, loadSummary]);

  return (
    <div className="cursor-file-changes-panel">
      <div className="panel-header">
        <h2>File Changes</h2>
        <div className="panel-controls">
          <label>
            Days:
            <input
              type="number"
              value={days}
              onChange={(e) => setDays(parseInt(e.target.value) || 7)}
              min={1}
              max={30}
            />
          </label>
          <label>
            Limit:
            <input
              type="number"
              value={limit}
              onChange={(e) => setLimit(parseInt(e.target.value) || 50)}
              min={10}
              max={200}
            />
          </label>
          <button className="btn btn-secondary" onClick={loadChanges} disabled={loading}>
            {loading ? 'Loading...' : 'Refresh'}
          </button>
        </div>
      </div>

      {summary && (
        <div className="changes-summary">
          <h3>Summary (Last {days} days)</h3>
          <div className="summary-grid">
            <div className="summary-item">
              <span className="summary-label">Total Changes</span>
              <span className="summary-value">{summary.totalChanges.toLocaleString()}</span>
            </div>
            <div className="summary-item">
              <span className="summary-label">Files Changed</span>
              <span className="summary-value">{summary.filesChanged.toLocaleString()}</span>
            </div>
            <div className="summary-item">
              <span className="summary-label">Additions</span>
              <span className="summary-value additions">{summary.additions.toLocaleString()}</span>
            </div>
            <div className="summary-item">
              <span className="summary-label">Deletions</span>
              <span className="summary-value deletions">{summary.deletions.toLocaleString()}</span>
            </div>
          </div>
          {Object.keys(summary.byType).length > 0 && (
            <div className="changes-by-type">
              <h4>By File Type</h4>
              <div className="type-list">
                {Object.entries(summary.byType)
                  .sort((a, b) => b[1] - a[1])
                  .map(([type, count]) => (
                    <div key={type} className="type-item">
                      <span className="type-name">{type || 'other'}</span>
                      <span className="type-count">{count}</span>
                    </div>
                  ))}
              </div>
            </div>
          )}
        </div>
      )}

      <div className="changes-list">
        <h3>Recent Changes ({changes.length})</h3>
        {loading && changes.length === 0 ? (
          <p>Loading changes...</p>
        ) : changes.length > 0 ? (
          <div className="changes-items">
            {changes.map((change, idx) => (
              <div key={idx} className={`change-item change-${change.type}`}>
                <div className="change-header">
                  <span className="change-type">{change.type}</span>
                  <span className="change-path">{change.path}</span>
                  <span className="change-time">
                    {new Date(change.timestamp).toLocaleString()}
                  </span>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No changes found</p>
        )}
      </div>
    </div>
  );
}

