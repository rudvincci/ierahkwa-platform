/**
 * Cursor Project Panel
 * Displays Cursor project information
 */

import { useState, useEffect, useCallback } from 'react';
import './CursorProjectPanel.css';

interface ProjectInfo {
  id: string;
  name: string;
  path: string;
  files?: string[];
  structure?: Record<string, any>;
  metadata?: Record<string, any>;
}

interface ProjectStats {
  totalFiles: number;
  totalLines: number;
  languages: Record<string, number>;
  lastModified: Date | string;
}

export default function CursorProjectPanel() {
  const [project, setProject] = useState<ProjectInfo | null>(null);
  const [stats, setStats] = useState<ProjectStats | null>(null);
  const [loading, setLoading] = useState(false);

  const loadProject = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch('/api/cursor/project');
      const data = await response.json();
      setProject(data.project);
      setStats(data.stats);
    } catch (error) {
      console.error('Failed to load project:', error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadProject();
    const interval = setInterval(loadProject, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, [loadProject]);

  if (loading && !project) {
    return <div className="cursor-project-panel">Loading project information...</div>;
  }

  if (!project) {
    return (
      <div className="cursor-project-panel">
        <p>No project information available</p>
        <button className="btn btn-primary" onClick={loadProject}>
          Refresh
        </button>
      </div>
    );
  }

  return (
    <div className="cursor-project-panel">
      <div className="project-header">
        <h2>Project: {project.name}</h2>
        <button className="btn btn-secondary" onClick={loadProject} disabled={loading}>
          {loading ? 'Refreshing...' : 'Refresh'}
        </button>
      </div>

      <div className="project-info">
        <div className="info-item">
          <span className="info-label">ID:</span>
          <span className="info-value">{project.id}</span>
        </div>
        <div className="info-item">
          <span className="info-label">Path:</span>
          <span className="info-value">{project.path}</span>
        </div>
      </div>

      {stats && (
        <div className="project-stats">
          <h3>Statistics</h3>
          <div className="stats-grid">
            <div className="stat-item">
              <span className="stat-label">Total Files</span>
              <span className="stat-value">{stats.totalFiles.toLocaleString()}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">Total Lines</span>
              <span className="stat-value">{stats.totalLines.toLocaleString()}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">Last Modified</span>
              <span className="stat-value">
                {new Date(stats.lastModified).toLocaleString()}
              </span>
            </div>
          </div>

          {Object.keys(stats.languages).length > 0 && (
            <div className="languages-section">
              <h4>Languages</h4>
              <div className="languages-list">
                {Object.entries(stats.languages)
                  .sort((a, b) => b[1] - a[1])
                  .map(([lang, count]) => (
                    <div key={lang} className="language-item">
                      <span className="language-name">{lang}</span>
                      <span className="language-count">{count}</span>
                    </div>
                  ))}
              </div>
            </div>
          )}
        </div>
      )}

      {project.files && project.files.length > 0 && (
        <div className="project-files">
          <h3>Files ({project.files.length})</h3>
          <div className="files-list">
            {project.files.slice(0, 50).map((file, idx) => (
              <div key={idx} className="file-item">{file}</div>
            ))}
            {project.files.length > 50 && (
              <div className="file-more">... and {project.files.length - 50} more files</div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

