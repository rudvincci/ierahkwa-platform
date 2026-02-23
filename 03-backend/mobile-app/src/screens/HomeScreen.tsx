/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  HOME SCREEN - Dashboard
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import React, { useEffect, useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { Text, Surface, Card, Avatar, Button } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useAuthStore } from '../store/authStore';
import { walletAPI, tokensAPI } from '../services/api';

export default function HomeScreen({ navigation }: any) {
  const { user } = useAuthStore();
  const [balance, setBalance] = useState('0.00');
  const [tokens, setTokens] = useState<any[]>([]);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [balanceRes, tokensRes] = await Promise.all([
        walletAPI.getBalance(),
        tokensAPI.getAll(),
      ]);
      setBalance(balanceRes.data.data?.balance || '0.00');
      setTokens(tokensRes.data.data || []);
    } catch (error) {
      console.error('Load data error:', error);
    }
  };

  const onRefresh = async () => {
    setRefreshing(true);
    await loadData();
    setRefreshing(false);
  };

  const quickActions = [
    { icon: 'send', label: 'Send', screen: 'Send', color: '#00FF41' },
    { icon: 'qrcode-scan', label: 'Scan', screen: 'ScanQR', color: '#00FFFF' },
    { icon: 'download', label: 'Receive', screen: 'Receive', color: '#FFD700' },
    { icon: 'robot', label: 'AI', screen: 'AIAssistant', color: '#9D4EDD' },
  ];

  const services = [
    { icon: 'bank', label: 'Banking', screen: 'Wallet' },
    { icon: 'file-document', label: 'Documents', screen: 'Government' },
    { icon: 'vote', label: 'Voting', screen: 'Services' },
    { icon: 'shield-check', label: 'Security', screen: 'Settings' },
  ];

  return (
    <ScrollView
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
      }
    >
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.greeting}>Welcome back,</Text>
          <Text style={styles.userName}>
            {user?.firstName} {user?.lastName}
          </Text>
        </View>
        <TouchableOpacity onPress={() => navigation.navigate('Profile')}>
          <Avatar.Text
            size={48}
            label={`${user?.firstName?.[0] || 'U'}${user?.lastName?.[0] || ''}`}
            style={styles.avatar}
          />
        </TouchableOpacity>
      </View>

      {/* Balance Card */}
      <Surface style={styles.balanceCard}>
        <Text style={styles.balanceLabel}>Total Balance</Text>
        <Text style={styles.balanceAmount}>
          <Text style={styles.currencySymbol}>ISB </Text>
          {parseFloat(balance).toLocaleString()}
        </Text>
        <View style={styles.walletInfo}>
          <Icon name="wallet" size={16} color="#888" />
          <Text style={styles.walletAddress}>
            {user?.walletAddress?.slice(0, 10)}...{user?.walletAddress?.slice(-6)}
          </Text>
        </View>
      </Surface>

      {/* Quick Actions */}
      <View style={styles.quickActions}>
        {quickActions.map((action, index) => (
          <TouchableOpacity
            key={index}
            style={styles.actionButton}
            onPress={() => navigation.navigate(action.screen)}
          >
            <View style={[styles.actionIcon, { backgroundColor: action.color + '20' }]}>
              <Icon name={action.icon} size={24} color={action.color} />
            </View>
            <Text style={styles.actionLabel}>{action.label}</Text>
          </TouchableOpacity>
        ))}
      </View>

      {/* Tokens */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Your Tokens</Text>
        <ScrollView horizontal showsHorizontalScrollIndicator={false}>
          {tokens.slice(0, 5).map((token, index) => (
            <Surface key={index} style={styles.tokenCard}>
              <Text style={styles.tokenSymbol}>{token.symbol}</Text>
              <Text style={styles.tokenBalance}>
                {parseFloat(token.balance || 0).toLocaleString()}
              </Text>
              <Text style={styles.tokenName}>{token.name}</Text>
            </Surface>
          ))}
        </ScrollView>
      </View>

      {/* Services Grid */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Quick Services</Text>
        <View style={styles.servicesGrid}>
          {services.map((service, index) => (
            <TouchableOpacity
              key={index}
              style={styles.serviceCard}
              onPress={() => navigation.navigate(service.screen)}
            >
              <Icon name={service.icon} size={28} color="#00FF41" />
              <Text style={styles.serviceLabel}>{service.label}</Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>

      {/* Citizen ID */}
      <Surface style={styles.citizenCard}>
        <View style={styles.citizenInfo}>
          <Icon name="card-account-details" size={24} color="#FFD700" />
          <View style={styles.citizenText}>
            <Text style={styles.citizenLabel}>Citizen ID</Text>
            <Text style={styles.citizenId}>{user?.citizenId}</Text>
          </View>
        </View>
        <Icon name="qrcode" size={32} color="#00FF41" />
      </Surface>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0A0A0F',
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  greeting: {
    color: '#888',
    fontSize: 14,
  },
  userName: {
    color: '#fff',
    fontSize: 24,
    fontWeight: 'bold',
  },
  avatar: {
    backgroundColor: '#00FF41',
  },
  balanceCard: {
    backgroundColor: '#1A1A2E',
    borderRadius: 16,
    padding: 24,
    marginBottom: 20,
  },
  balanceLabel: {
    color: '#888',
    fontSize: 14,
  },
  balanceAmount: {
    color: '#00FF41',
    fontSize: 36,
    fontWeight: 'bold',
    marginTop: 5,
  },
  currencySymbol: {
    fontSize: 20,
    color: '#888',
  },
  walletInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 15,
  },
  walletAddress: {
    color: '#888',
    fontSize: 12,
    marginLeft: 8,
  },
  quickActions: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 25,
  },
  actionButton: {
    alignItems: 'center',
  },
  actionIcon: {
    width: 56,
    height: 56,
    borderRadius: 16,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 8,
  },
  actionLabel: {
    color: '#fff',
    fontSize: 12,
  },
  section: {
    marginBottom: 25,
  },
  sectionTitle: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 15,
  },
  tokenCard: {
    backgroundColor: '#1A1A2E',
    borderRadius: 12,
    padding: 16,
    marginRight: 12,
    minWidth: 120,
  },
  tokenSymbol: {
    color: '#00FF41',
    fontSize: 16,
    fontWeight: 'bold',
  },
  tokenBalance: {
    color: '#fff',
    fontSize: 20,
    fontWeight: 'bold',
    marginTop: 5,
  },
  tokenName: {
    color: '#888',
    fontSize: 11,
    marginTop: 5,
  },
  servicesGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
  },
  serviceCard: {
    backgroundColor: '#1A1A2E',
    borderRadius: 12,
    padding: 20,
    width: '48%',
    alignItems: 'center',
    marginBottom: 12,
  },
  serviceLabel: {
    color: '#fff',
    marginTop: 10,
    fontSize: 14,
  },
  citizenCard: {
    backgroundColor: '#1A1A2E',
    borderRadius: 12,
    padding: 16,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 30,
  },
  citizenInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  citizenText: {
    marginLeft: 12,
  },
  citizenLabel: {
    color: '#888',
    fontSize: 12,
  },
  citizenId: {
    color: '#FFD700',
    fontSize: 14,
    fontWeight: 'bold',
  },
});
