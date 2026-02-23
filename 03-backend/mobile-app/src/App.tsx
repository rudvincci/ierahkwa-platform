// ============================================================================
// IERAHKWA SOVEREIGN PLATFORM - MOBILE APP
// React Native application with full banking features
// ============================================================================

import React, { useState, useEffect, createContext, useContext } from 'react';
import {
  SafeAreaView,
  ScrollView,
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  StatusBar,
  Alert,
  ActivityIndicator,
  RefreshControl,
  Dimensions,
  Platform,
} from 'react-native';

// Types
interface User {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
}

interface Account {
  id: string;
  accountNumber: string;
  accountType: string;
  balance: number;
  currency: string;
  isVip: boolean;
}

interface Transaction {
  id: string;
  transactionCode: string;
  amount: number;
  type: string;
  status: string;
  description: string;
  createdAt: string;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

// API Configuration
const API_BASE = __DEV__ ? 'http://localhost:5000/api' : 'https://api.ierahkwa.gov/api';

// Auth Context
const AuthContext = createContext<AuthContextType | null>(null);

const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

// API Service
class ApiService {
  private token: string | null = null;

  setToken(token: string | null) {
    this.token = token;
  }

  private async request(endpoint: string, options: RequestInit = {}) {
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
      ...(this.token && { Authorization: `Bearer ${this.token}` }),
    };

    const response = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers: { ...headers, ...options.headers },
    });

    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.error || 'Request failed');
    }

    return data;
  }

  // Auth
  async login(email: string, password: string) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  }

  async register(email: string, password: string, fullName: string) {
    return this.request('/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, password, fullName }),
    });
  }

  async getProfile() {
    return this.request('/auth/me');
  }

  // Banking
  async getAccounts() {
    return this.request('/accounts');
  }

  async getAccount(id: string) {
    return this.request(`/accounts/${id}`);
  }

  async getTransactions(accountId?: string) {
    const endpoint = accountId ? `/transactions?accountId=${accountId}` : '/transactions';
    return this.request(endpoint);
  }

  async transfer(fromAccountId: string, toAccountId: string, amount: number, description?: string) {
    return this.request('/transactions/transfer', {
      method: 'POST',
      body: JSON.stringify({ fromAccountId, toAccountId, amount, description }),
    });
  }

  // Trading
  async getMarkets() {
    return this.request('/futures/markets');
  }

  async getPositions() {
    return this.request('/futures/positions');
  }

  async placeOrder(order: any) {
    return this.request('/futures/order', {
      method: 'POST',
      body: JSON.stringify(order),
    });
  }

  // Biometrics
  async biometricLogin(userId: string, biometricData: string, type: string, deviceId: string) {
    return this.request('/biometrics/login', {
      method: 'POST',
      body: JSON.stringify({ userId, biometricData, type, deviceId }),
    });
  }
}

const api = new ApiService();

// Theme
const colors = {
  primary: '#c9a227',
  secondary: '#1a1f2e',
  background: '#0a0e17',
  surface: '#141922',
  text: '#ffffff',
  textSecondary: '#8892a4',
  success: '#22c55e',
  error: '#ef4444',
  warning: '#f59e0b',
  border: '#2a3441',
};

// Components
const Header: React.FC<{ title: string; showBack?: boolean; onBack?: () => void }> = ({ 
  title, 
  showBack, 
  onBack 
}) => (
  <View style={styles.header}>
    {showBack && (
      <TouchableOpacity onPress={onBack} style={styles.backButton}>
        <Text style={styles.backText}>←</Text>
      </TouchableOpacity>
    )}
    <Text style={styles.headerTitle}>{title}</Text>
    <View style={styles.headerRight} />
  </View>
);

const Card: React.FC<{ children: React.ReactNode; style?: any }> = ({ children, style }) => (
  <View style={[styles.card, style]}>{children}</View>
);

const Button: React.FC<{
  title: string;
  onPress: () => void;
  variant?: 'primary' | 'secondary' | 'outline';
  loading?: boolean;
  disabled?: boolean;
}> = ({ title, onPress, variant = 'primary', loading, disabled }) => (
  <TouchableOpacity
    style={[
      styles.button,
      variant === 'secondary' && styles.buttonSecondary,
      variant === 'outline' && styles.buttonOutline,
      disabled && styles.buttonDisabled,
    ]}
    onPress={onPress}
    disabled={disabled || loading}
  >
    {loading ? (
      <ActivityIndicator color={variant === 'outline' ? colors.primary : colors.background} />
    ) : (
      <Text
        style={[
          styles.buttonText,
          variant === 'outline' && styles.buttonTextOutline,
        ]}
      >
        {title}
      </Text>
    )}
  </TouchableOpacity>
);

const Input: React.FC<{
  placeholder: string;
  value: string;
  onChangeText: (text: string) => void;
  secureTextEntry?: boolean;
  keyboardType?: 'default' | 'email-address' | 'numeric';
}> = ({ placeholder, value, onChangeText, secureTextEntry, keyboardType }) => (
  <TextInput
    style={styles.input}
    placeholder={placeholder}
    placeholderTextColor={colors.textSecondary}
    value={value}
    onChangeText={onChangeText}
    secureTextEntry={secureTextEntry}
    keyboardType={keyboardType}
    autoCapitalize="none"
  />
);

// Screens
const LoginScreen: React.FC<{ onNavigate: (screen: string) => void }> = ({ onNavigate }) => {
  const { login, isLoading } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert('Error', 'Please enter email and password');
      return;
    }

    try {
      await login(email, password);
    } catch (error: any) {
      Alert.alert('Login Failed', error.message);
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.loginContainer}>
        <Text style={styles.logo}>🪶</Text>
        <Text style={styles.title}>IERAHKWA</Text>
        <Text style={styles.subtitle}>Sovereign Banking Platform</Text>

        <View style={styles.form}>
          <Input
            placeholder="Email"
            value={email}
            onChangeText={setEmail}
            keyboardType="email-address"
          />
          <Input
            placeholder="Password"
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />
          <Button title="Login" onPress={handleLogin} loading={isLoading} />
          <Button
            title="Create Account"
            onPress={() => onNavigate('register')}
            variant="outline"
          />
        </View>

        <TouchableOpacity style={styles.biometricButton}>
          <Text style={styles.biometricText}>👆 Login with Biometrics</Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
};

const DashboardScreen: React.FC<{ onNavigate: (screen: string) => void }> = ({ onNavigate }) => {
  const { user, logout } = useAuth();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [refreshing, setRefreshing] = useState(false);
  const [loading, setLoading] = useState(true);

  const loadData = async () => {
    try {
      const [accountsRes, txRes] = await Promise.all([
        api.getAccounts(),
        api.getTransactions(),
      ]);
      setAccounts(accountsRes.data || []);
      setTransactions(txRes.data || []);
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const onRefresh = () => {
    setRefreshing(true);
    loadData();
  };

  const totalBalance = accounts.reduce((sum, acc) => sum + acc.balance, 0);

  if (loading) {
    return (
      <View style={[styles.container, styles.center]}>
        <ActivityIndicator size="large" color={colors.primary} />
      </View>
    );
  }

  return (
    <SafeAreaView style={styles.container}>
      <Header title="Dashboard" />
      <ScrollView
        style={styles.content}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={colors.primary} />
        }
      >
        {/* Welcome */}
        <Card>
          <Text style={styles.welcomeText}>Welcome back,</Text>
          <Text style={styles.userName}>{user?.fullName}</Text>
        </Card>

        {/* Total Balance */}
        <Card style={styles.balanceCard}>
          <Text style={styles.balanceLabel}>Total Balance</Text>
          <Text style={styles.balanceAmount}>
            ${totalBalance.toLocaleString('en-US', { minimumFractionDigits: 2 })}
          </Text>
          <View style={styles.balanceActions}>
            <TouchableOpacity
              style={styles.actionButton}
              onPress={() => onNavigate('transfer')}
            >
              <Text style={styles.actionIcon}>↗️</Text>
              <Text style={styles.actionText}>Send</Text>
            </TouchableOpacity>
            <TouchableOpacity style={styles.actionButton}>
              <Text style={styles.actionIcon}>↙️</Text>
              <Text style={styles.actionText}>Receive</Text>
            </TouchableOpacity>
            <TouchableOpacity
              style={styles.actionButton}
              onPress={() => onNavigate('trading')}
            >
              <Text style={styles.actionIcon}>📈</Text>
              <Text style={styles.actionText}>Trade</Text>
            </TouchableOpacity>
          </View>
        </Card>

        {/* Accounts */}
        <Text style={styles.sectionTitle}>Accounts</Text>
        {accounts.map((account) => (
          <Card key={account.id} style={styles.accountCard}>
            <View style={styles.accountHeader}>
              <Text style={styles.accountType}>{account.accountType}</Text>
              {account.isVip && <Text style={styles.vipBadge}>VIP</Text>}
            </View>
            <Text style={styles.accountNumber}>{account.accountNumber}</Text>
            <Text style={styles.accountBalance}>
              {account.currency} {account.balance.toLocaleString('en-US', { minimumFractionDigits: 2 })}
            </Text>
          </Card>
        ))}

        {/* Recent Transactions */}
        <Text style={styles.sectionTitle}>Recent Transactions</Text>
        {transactions.slice(0, 5).map((tx) => (
          <Card key={tx.id} style={styles.txCard}>
            <View style={styles.txRow}>
              <View>
                <Text style={styles.txDescription}>{tx.description || tx.type}</Text>
                <Text style={styles.txDate}>
                  {new Date(tx.createdAt).toLocaleDateString()}
                </Text>
              </View>
              <Text
                style={[
                  styles.txAmount,
                  tx.type === 'credit' ? styles.txCredit : styles.txDebit,
                ]}
              >
                {tx.type === 'credit' ? '+' : '-'}${Math.abs(tx.amount).toFixed(2)}
              </Text>
            </View>
          </Card>
        ))}

        {/* Logout */}
        <Button title="Logout" onPress={logout} variant="outline" />
        <View style={{ height: 40 }} />
      </ScrollView>
    </SafeAreaView>
  );
};

const TradingScreen: React.FC<{ onNavigate: (screen: string) => void }> = ({ onNavigate }) => {
  const [markets, setMarkets] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMarkets();
  }, []);

  const loadMarkets = async () => {
    try {
      const res = await api.getMarkets();
      setMarkets(res.data || []);
    } catch (error) {
      console.error('Failed to load markets:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      <Header title="Trading" showBack onBack={() => onNavigate('dashboard')} />
      <ScrollView style={styles.content}>
        <Text style={styles.sectionTitle}>Markets</Text>
        {loading ? (
          <ActivityIndicator color={colors.primary} />
        ) : (
          markets.map((market) => (
            <Card key={market.symbol} style={styles.marketCard}>
              <View style={styles.marketRow}>
                <View>
                  <Text style={styles.marketSymbol}>{market.symbol}</Text>
                  <Text style={styles.marketVolume}>Vol: ${market.volume24h?.toLocaleString()}</Text>
                </View>
                <View style={styles.marketPriceContainer}>
                  <Text style={styles.marketPrice}>${market.lastPrice?.toLocaleString()}</Text>
                  <Text
                    style={[
                      styles.marketChange,
                      market.changePercent24h >= 0 ? styles.positive : styles.negative,
                    ]}
                  >
                    {market.changePercent24h >= 0 ? '+' : ''}{market.changePercent24h?.toFixed(2)}%
                  </Text>
                </View>
              </View>
            </Card>
          ))
        )}
      </ScrollView>
    </SafeAreaView>
  );
};

// Main App
const App: React.FC = () => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [currentScreen, setCurrentScreen] = useState('login');

  const login = async (email: string, password: string) => {
    setIsLoading(true);
    try {
      const response = await api.login(email, password);
      const { user: userData, tokens } = response.data;
      
      setUser(userData);
      setToken(tokens.accessToken);
      api.setToken(tokens.accessToken);
      setCurrentScreen('dashboard');
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    api.setToken(null);
    setCurrentScreen('login');
  };

  const authValue: AuthContextType = {
    user,
    token,
    login,
    logout,
    isLoading,
  };

  const renderScreen = () => {
    switch (currentScreen) {
      case 'login':
        return <LoginScreen onNavigate={setCurrentScreen} />;
      case 'dashboard':
        return <DashboardScreen onNavigate={setCurrentScreen} />;
      case 'trading':
        return <TradingScreen onNavigate={setCurrentScreen} />;
      default:
        return <LoginScreen onNavigate={setCurrentScreen} />;
    }
  };

  return (
    <AuthContext.Provider value={authValue}>
      <StatusBar barStyle="light-content" backgroundColor={colors.background} />
      {renderScreen()}
    </AuthContext.Provider>
  );
};

// Styles
const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background,
  },
  center: {
    justifyContent: 'center',
    alignItems: 'center',
  },
  content: {
    flex: 1,
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
  },
  headerTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: colors.text,
  },
  backButton: {
    padding: 8,
  },
  backText: {
    fontSize: 24,
    color: colors.primary,
  },
  headerRight: {
    width: 40,
  },
  loginContainer: {
    flex: 1,
    justifyContent: 'center',
    padding: 24,
  },
  logo: {
    fontSize: 64,
    textAlign: 'center',
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: colors.primary,
    textAlign: 'center',
    marginTop: 16,
  },
  subtitle: {
    fontSize: 16,
    color: colors.textSecondary,
    textAlign: 'center',
    marginBottom: 40,
  },
  form: {
    gap: 16,
  },
  input: {
    backgroundColor: colors.surface,
    borderWidth: 1,
    borderColor: colors.border,
    borderRadius: 12,
    padding: 16,
    fontSize: 16,
    color: colors.text,
  },
  button: {
    backgroundColor: colors.primary,
    borderRadius: 12,
    padding: 16,
    alignItems: 'center',
  },
  buttonSecondary: {
    backgroundColor: colors.secondary,
  },
  buttonOutline: {
    backgroundColor: 'transparent',
    borderWidth: 1,
    borderColor: colors.primary,
  },
  buttonDisabled: {
    opacity: 0.5,
  },
  buttonText: {
    fontSize: 16,
    fontWeight: '600',
    color: colors.background,
  },
  buttonTextOutline: {
    color: colors.primary,
  },
  biometricButton: {
    marginTop: 24,
    alignItems: 'center',
  },
  biometricText: {
    color: colors.textSecondary,
    fontSize: 16,
  },
  card: {
    backgroundColor: colors.surface,
    borderRadius: 16,
    padding: 16,
    marginBottom: 12,
  },
  welcomeText: {
    color: colors.textSecondary,
    fontSize: 14,
  },
  userName: {
    color: colors.text,
    fontSize: 24,
    fontWeight: 'bold',
  },
  balanceCard: {
    backgroundColor: colors.primary,
  },
  balanceLabel: {
    color: colors.background,
    fontSize: 14,
    opacity: 0.8,
  },
  balanceAmount: {
    color: colors.background,
    fontSize: 36,
    fontWeight: 'bold',
    marginVertical: 8,
  },
  balanceActions: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    marginTop: 16,
  },
  actionButton: {
    alignItems: 'center',
  },
  actionIcon: {
    fontSize: 24,
    marginBottom: 4,
  },
  actionText: {
    color: colors.background,
    fontSize: 12,
    fontWeight: '600',
  },
  sectionTitle: {
    color: colors.text,
    fontSize: 18,
    fontWeight: '600',
    marginVertical: 16,
  },
  accountCard: {
    marginBottom: 8,
  },
  accountHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  accountType: {
    color: colors.textSecondary,
    fontSize: 12,
    textTransform: 'uppercase',
  },
  vipBadge: {
    backgroundColor: colors.primary,
    color: colors.background,
    fontSize: 10,
    fontWeight: 'bold',
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 4,
  },
  accountNumber: {
    color: colors.text,
    fontSize: 14,
    fontFamily: Platform.OS === 'ios' ? 'Menlo' : 'monospace',
    marginVertical: 4,
  },
  accountBalance: {
    color: colors.text,
    fontSize: 20,
    fontWeight: 'bold',
  },
  txCard: {
    marginBottom: 8,
  },
  txRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  txDescription: {
    color: colors.text,
    fontSize: 14,
  },
  txDate: {
    color: colors.textSecondary,
    fontSize: 12,
    marginTop: 2,
  },
  txAmount: {
    fontSize: 16,
    fontWeight: '600',
  },
  txCredit: {
    color: colors.success,
  },
  txDebit: {
    color: colors.error,
  },
  marketCard: {
    marginBottom: 8,
  },
  marketRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  marketSymbol: {
    color: colors.text,
    fontSize: 16,
    fontWeight: '600',
  },
  marketVolume: {
    color: colors.textSecondary,
    fontSize: 12,
    marginTop: 2,
  },
  marketPriceContainer: {
    alignItems: 'flex-end',
  },
  marketPrice: {
    color: colors.text,
    fontSize: 16,
    fontWeight: '600',
  },
  marketChange: {
    fontSize: 14,
    marginTop: 2,
  },
  positive: {
    color: colors.success,
  },
  negative: {
    color: colors.error,
  },
});

export default App;
