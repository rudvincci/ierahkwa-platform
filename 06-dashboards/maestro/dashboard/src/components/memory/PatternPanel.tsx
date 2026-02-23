/**
 * Pattern Panel
 * Displays pattern browsing and extraction
 */

import { useState, useEffect, useCallback } from 'react';
import './PatternPanel.css';

interface Pattern {
  id: string;
  title: string;
  metadata: {
    pattern_id: string;
    category: string;
    frequency: number;
    confidence: number;
    status: string;
  };
  content: string;
  filePath: string;
}

export default function PatternPanel() {
  const [patterns, setPatterns] = useState<Pattern[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [loading, setLoading] = useState(false);
  const [extracting, setExtracting] = useState(false);

  const loadPatterns = useCallback(async () => {
    setLoading(true);
    try {
      const url = searchQuery
        ? `/api/memory/patterns?q=${encodeURIComponent(searchQuery)}`
        : '/api/memory/patterns';
      const response = await fetch(url);
      const data = await response.json();
      setPatterns(data.patterns || []);
    } catch (error) {
      console.error('Failed to load patterns:', error);
    } finally {
      setLoading(false);
    }
  }, [searchQuery]);

  const handleExtractPatterns = useCallback(async () => {
    setExtracting(true);
    try {
      const response = await fetch('/api/memory/patterns/extract', {
        method: 'POST',
      });
      const data = await response.json();
      if (data.success) {
        alert(`Extracted ${data.count} patterns successfully!`);
        loadPatterns();
      } else {
        alert('Failed to extract patterns');
      }
    } catch (error) {
      console.error('Failed to extract patterns:', error);
      alert('Failed to extract patterns');
    } finally {
      setExtracting(false);
    }
  }, [loadPatterns]);

  useEffect(() => {
    loadPatterns();
  }, [loadPatterns]);

  return (
    <div className="pattern-panel">
      <div className="pattern-header">
        <h2>Pattern Extraction</h2>
        <div className="pattern-controls">
          <input
            type="text"
            placeholder="Search patterns..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && loadPatterns()}
          />
          <button
            className="btn btn-primary"
            onClick={handleExtractPatterns}
            disabled={extracting}
          >
            {extracting ? 'Extracting...' : 'Extract Patterns'}
          </button>
        </div>
      </div>

      {loading ? (
        <p>Loading patterns...</p>
      ) : patterns.length > 0 ? (
        <div className="patterns-list">
          {patterns.map((pattern) => (
            <div key={pattern.id} className="pattern-item">
              <div className="pattern-header-item">
                <h3>{pattern.title}</h3>
                <div className="pattern-badges">
                  <span className={`badge status-${pattern.metadata.status}`}>
                    {pattern.metadata.status}
                  </span>
                  <span className="badge category">{pattern.metadata.category}</span>
                  <span className="badge frequency">
                    {pattern.metadata.frequency} occurrences
                  </span>
                  <span className="badge confidence">
                    {pattern.metadata.confidence}% confidence
                  </span>
                </div>
              </div>
              <div className="pattern-content">
                {pattern.content.substring(0, 500)}
                {pattern.content.length > 500 && '...'}
              </div>
              <div className="pattern-footer">
                <span className="pattern-id">ID: {pattern.metadata.pattern_id}</span>
                <span className="pattern-path">{pattern.filePath}</span>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p>No patterns found</p>
      )}
    </div>
  );
}

