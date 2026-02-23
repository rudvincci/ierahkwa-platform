import './StatusBadge.css';

interface StatusBadgeProps {
  status: 'running' | 'completed' | 'failed' | 'paused' | 'stopped';
}

export default function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <span className={`status-badge status-${status}`}>
      {status.toUpperCase()}
    </span>
  );
}
