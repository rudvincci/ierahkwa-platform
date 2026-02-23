import './Header.css';

interface HeaderProps {
  isConnected: boolean;
}

export default function Header({ isConnected }: HeaderProps) {
  return (
    <div className="header">
      <h1>Maestro Dashboard</h1>
      <div className="status">
        {isConnected ? '🟢 Connected' : '🔴 Disconnected'}
      </div>
    </div>
  );
}
