/**
 * Wallet Screen - Token Balances & Transactions
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
} from 'react-native';
import { useTranslation } from 'react-i18next';

export default function WalletScreen({ navigation }) {
  const { t } = useTranslation();
  const [balance, setBalance] = useState('125,000.00');
  const [tokens, setTokens] = useState([
    { symbol: 'IGT-MAIN', name: 'Ierahkwa Main', balance: '100,000', logo: '🏛️', value: '$100,000' },
    { symbol: 'IGT-STABLE', name: 'Ierahkwa Stable', balance: '25,000', logo: '💵', value: '$25,000' },
    { symbol: 'WETH', name: 'Wrapped Ether', balance: '2.5', logo: 'Ξ', value: '$6,250' },
    { symbol: 'USDT', name: 'Tether USD', balance: '5,000', logo: '💲', value: '$5,000' },
  ]);

  return (
    <ScrollView style={styles.container}>
      {/* Balance Card */}
      <View style={styles.balanceCard}>
        <Text style={styles.balanceLabel}>{t('total_balance')}</Text>
        <Text style={styles.balanceValue}>${balance}</Text>
        <Text style={styles.balanceChange}>+12.5% this week</Text>
        
        <View style={styles.actionButtons}>
          <TouchableOpacity style={styles.actionBtn}>
            <Text style={styles.actionIcon}>📤</Text>
            <Text style={styles.actionText}>Send</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionBtn}>
            <Text style={styles.actionIcon}>📥</Text>
            <Text style={styles.actionText}>Receive</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionBtn}>
            <Text style={styles.actionIcon}>💱</Text>
            <Text style={styles.actionText}>{t('swap')}</Text>
          </TouchableOpacity>
          <TouchableOpacity style={styles.actionBtn} onPress={() => navigation.navigate('Bridge')}>
            <Text style={styles.actionIcon}>🌉</Text>
            <Text style={styles.actionText}>{t('bridge')}</Text>
          </TouchableOpacity>
        </View>
      </View>

      {/* Search */}
      <View style={styles.searchContainer}>
        <TextInput
          style={styles.searchInput}
          placeholder="Search tokens..."
          placeholderTextColor="#666"
        />
      </View>

      {/* Token List */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Your Assets</Text>
        {tokens.map((token, i) => (
          <TouchableOpacity
            key={token.symbol}
            style={styles.tokenItem}
            onPress={() => navigation.navigate('TokenDetail', { token })}
          >
            <View style={styles.tokenInfo}>
              <Text style={styles.tokenLogo}>{token.logo}</Text>
              <View>
                <Text style={styles.tokenName}>{token.name}</Text>
                <Text style={styles.tokenSymbol}>{token.symbol}</Text>
              </View>
            </View>
            <View style={styles.tokenBalance}>
              <Text style={styles.balanceText}>{token.balance}</Text>
              <Text style={styles.valueText}>{token.value}</Text>
            </View>
          </TouchableOpacity>
        ))}
      </View>

      {/* Recent Transactions */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('recent_transactions')}</Text>
        <TransactionItem
          type="receive"
          amount="+1,000 IGT"
          address="0x1234...5678"
          time="2 hours ago"
        />
        <TransactionItem
          type="send"
          amount="-500 IGT"
          address="0xabcd...efgh"
          time="5 hours ago"
        />
        <TransactionItem
          type="swap"
          amount="ETH → IGT"
          address="TradeX"
          time="1 day ago"
        />
      </View>
    </ScrollView>
  );
}

function TransactionItem({ type, amount, address, time }) {
  const icons = { receive: '📥', send: '📤', swap: '💱' };
  const colors = { receive: '#00FF41', send: '#FF6B35', swap: '#00FFFF' };
  
  return (
    <View style={styles.txItem}>
      <View style={styles.txInfo}>
        <Text style={styles.txIcon}>{icons[type]}</Text>
        <View>
          <Text style={[styles.txAmount, { color: colors[type] }]}>{amount}</Text>
          <Text style={styles.txAddress}>{address}</Text>
        </View>
      </View>
      <Text style={styles.txTime}>{time}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0a0e17',
  },
  balanceCard: {
    margin: 15,
    padding: 25,
    backgroundColor: '#1a1f2e',
    borderRadius: 20,
    borderWidth: 1,
    borderColor: '#FFD700',
  },
  balanceLabel: {
    color: '#888',
    fontSize: 14,
  },
  balanceValue: {
    color: '#FFD700',
    fontSize: 36,
    fontWeight: 'bold',
    marginVertical: 5,
  },
  balanceChange: {
    color: '#00FF41',
    fontSize: 14,
  },
  actionButtons: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    marginTop: 25,
    paddingTop: 20,
    borderTopWidth: 1,
    borderTopColor: '#2a2f3e',
  },
  actionBtn: {
    alignItems: 'center',
  },
  actionIcon: {
    fontSize: 24,
    marginBottom: 5,
  },
  actionText: {
    color: '#fff',
    fontSize: 12,
  },
  searchContainer: {
    paddingHorizontal: 15,
    marginBottom: 10,
  },
  searchInput: {
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    color: '#fff',
    fontSize: 16,
  },
  section: {
    padding: 15,
  },
  sectionTitle: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 15,
  },
  tokenItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    marginBottom: 10,
  },
  tokenInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 12,
  },
  tokenLogo: {
    fontSize: 32,
  },
  tokenName: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 16,
  },
  tokenSymbol: {
    color: '#888',
    fontSize: 12,
  },
  tokenBalance: {
    alignItems: 'flex-end',
  },
  balanceText: {
    color: '#fff',
    fontWeight: 'bold',
    fontSize: 16,
  },
  valueText: {
    color: '#888',
    fontSize: 12,
  },
  txItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    marginBottom: 10,
  },
  txInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 12,
  },
  txIcon: {
    fontSize: 24,
  },
  txAmount: {
    fontWeight: '600',
  },
  txAddress: {
    color: '#888',
    fontSize: 12,
  },
  txTime: {
    color: '#666',
    fontSize: 12,
  },
});
