/**
 * Memory Panel
 * Displays memory search, add, and browse functionality
 */

import { useState, useEffect, useCallback } from 'react';
import './MemoryPanel.css';

interface MemoryEntry {
  content: string;
  type: string;
  role?: string;
  tags?: string[];
  public?: boolean;
  filePath: string;
  timestamp: Date | string;
}

interface SearchResult {
  file_path: string;
  score: number;
  hybrid_score?: number;
  semantic_score?: number;
  keyword_score?: number;
  source: 'semantic' | 'keyword' | 'hybrid';
  matched_terms?: string[];
}

export default function MemoryPanel() {
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [memoryEntries, setMemoryEntries] = useState<MemoryEntry[]>([]);
  const [loading, setLoading] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [newMemory, setNewMemory] = useState({
    content: '',
    type: 'mid-term' as 'short-term' | 'mid-term' | 'long-term',
    role: '',
    tags: '',
    public: true,
  });

  const handleSearch = useCallback(async () => {
    if (!searchQuery.trim()) return;

    setLoading(true);
    try {
      const response = await fetch(`/api/memory/search?q=${encodeURIComponent(searchQuery)}&top_k=10`);
      const data = await response.json();
      setSearchResults(data.results || []);
    } catch (error) {
      console.error('Failed to search memory:', error);
    } finally {
      setLoading(false);
    }
  }, [searchQuery]);

  const handleAddMemory = useCallback(async () => {
    if (!newMemory.content.trim()) return;

    setLoading(true);
    try {
      const tags = newMemory.tags.split(',').map(t => t.trim()).filter(t => t);
      const response = await fetch('/api/memory/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          content: newMemory.content,
          type: newMemory.type,
          role: newMemory.role || undefined,
          tags: tags.length > 0 ? tags : undefined,
          public: newMemory.public,
        }),
      });

      if (response.ok) {
        alert('Memory added successfully!');
        setNewMemory({ content: '', type: 'mid-term', role: '', tags: '', public: true });
        setShowAddForm(false);
        handleSearch(); // Refresh search
      } else {
        const error = await response.json();
        alert(`Failed to add memory: ${error.error}`);
      }
    } catch (error) {
      console.error('Failed to add memory:', error);
      alert('Failed to add memory');
    } finally {
      setLoading(false);
    }
  }, [newMemory, handleSearch]);

  const loadMemoryEntries = useCallback(async () => {
    try {
      const response = await fetch('/api/memory/list');
      const data = await response.json();
      setMemoryEntries(data.entries || []);
    } catch (error) {
      console.error('Failed to load memory entries:', error);
    }
  }, []);

  useEffect(() => {
    loadMemoryEntries();
  }, [loadMemoryEntries]);

  return (
    <div className="memory-panel">
      <div className="memory-header">
        <h2>Memory Management</h2>
        <button
          className="btn btn-primary"
          onClick={() => setShowAddForm(!showAddForm)}
        >
          {showAddForm ? 'Cancel' : 'Add Memory'}
        </button>
      </div>

      {showAddForm && (
        <div className="memory-add-form">
          <h3>Add New Memory</h3>
          <textarea
            placeholder="Memory content..."
            value={newMemory.content}
            onChange={(e) => setNewMemory({ ...newMemory, content: e.target.value })}
            rows={5}
          />
          <div className="form-row">
            <label>
              Type:
              <select
                value={newMemory.type}
                onChange={(e) => setNewMemory({ ...newMemory, type: e.target.value as any })}
              >
                <option value="short-term">Short-term</option>
                <option value="mid-term">Mid-term</option>
                <option value="long-term">Long-term</option>
              </select>
            </label>
            <label>
              Role:
              <input
                type="text"
                placeholder="dev, ops, sec, ai"
                value={newMemory.role}
                onChange={(e) => setNewMemory({ ...newMemory, role: e.target.value })}
              />
            </label>
            <label>
              Tags (comma-separated):
              <input
                type="text"
                placeholder="tag1, tag2"
                value={newMemory.tags}
                onChange={(e) => setNewMemory({ ...newMemory, tags: e.target.value })}
              />
            </label>
            <label>
              <input
                type="checkbox"
                checked={newMemory.public}
                onChange={(e) => setNewMemory({ ...newMemory, public: e.target.checked })}
              />
              Public
            </label>
          </div>
          <button
            className="btn btn-primary"
            onClick={handleAddMemory}
            disabled={loading || !newMemory.content.trim()}
          >
            {loading ? 'Adding...' : 'Add Memory'}
          </button>
        </div>
      )}

      <div className="memory-search">
        <h3>Search Memory</h3>
        <div className="search-controls">
          <input
            type="text"
            placeholder="Search memory..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
          />
          <button
            className="btn btn-primary"
            onClick={handleSearch}
            disabled={loading || !searchQuery.trim()}
          >
            {loading ? 'Searching...' : 'Search'}
          </button>
        </div>

        {searchResults.length > 0 && (
          <div className="search-results">
            <h4>Results ({searchResults.length})</h4>
            {searchResults.map((result, idx) => (
              <div key={idx} className="search-result-item">
                <div className="result-header">
                  <span className="result-path">{result.file_path}</span>
                  <span className="result-score">
                    Score: {result.hybrid_score?.toFixed(3) || result.score.toFixed(3)}
                  </span>
                </div>
                <div className="result-details">
                  <span className="result-source">{result.source}</span>
                  {result.matched_terms && result.matched_terms.length > 0 && (
                    <span className="result-terms">
                      Terms: {result.matched_terms.join(', ')}
                    </span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="memory-entries">
        <h3>Recent Memory Entries</h3>
        {memoryEntries.length > 0 ? (
          <div className="entries-list">
            {memoryEntries.slice(0, 10).map((entry, idx) => (
              <div key={idx} className="memory-entry-item">
                <div className="entry-header">
                  <span className="entry-type">{entry.type}</span>
                  {entry.role && <span className="entry-role">{entry.role}</span>}
                  {entry.tags && entry.tags.length > 0 && (
                    <span className="entry-tags">
                      {entry.tags.map(tag => `#${tag}`).join(' ')}
                    </span>
                  )}
                </div>
                <div className="entry-content">{entry.content.substring(0, 200)}...</div>
                <div className="entry-footer">
                  <span className="entry-path">{entry.filePath}</span>
                  <span className="entry-timestamp">
                    {new Date(entry.timestamp).toLocaleString()}
                  </span>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p>No memory entries found</p>
        )}
      </div>
    </div>
  );
}

