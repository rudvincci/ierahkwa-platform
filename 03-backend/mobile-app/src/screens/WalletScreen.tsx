/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  WALLET SCREEN
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import React, { useEffect, useState } from 'react';
import { View, StyleSheet, ScrollView, TouchableOpacity, FlatList } from 'react-native';
import { Text, Surface, Button, Divider } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { walletAPI } from '../services/api';

export default function WalletScreen({ navigation }: any) {
  const [balance, setBalance] = useState('0.00');
  const [transactions, setTransactions] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadWalletData();
  }, []);

  const loadWalletData = async () => {
    try {
      const [balanceRes, txRes] = await Promise.all([
        walletAPI.getBalance(),
        walletAPI.getTransactions({ limit: 10 }),
      ]);
      setBalance(balanceRes.data.data?.balance || '0.00');
      setTransactions(txRes.data.data || []);
    } catch (error) {
      console.error('Wallet data error:', error);
    } finally {
      setLoading(false);
    }
  };

  const renderTransaction = ({ item }: any) => (
    <TouchableOpacity style={styles.txItem}>
      <View style={[styles.txIcon, { backgroundColor: item.type === 'receive' ? '#00FF4120' : '#FF444420' }]}>
        <Icon
          name={item.type === 'receive' ? 'arrow-down' : 'arrow-up'}
          size={20}
          color={item.type === 'receive' ? '#00FF41' : '#FF4444'}
        />
      </View>
      <View style={styles.txInfo}>
        <Text style={styles.txTitle}>{item.type === 'receive' ? 'Received' : 'Sent'}</Text>
        <Text style={styles.txAddress}>{item.from?.slice(0, 10)}...{item.from?.slice(-6)}</Text>
      </View>
      <View style={styles.txAmount}>
        <Text style={[styles.txValue, { color: item.type === 'receive' ? '#00FF41' : '#FF4444' }]}>
          {item.type === 'receive' ? '+' : '-'}{parseFloat(item.amount).toLocaleString()}
        </Text>
        <Text style={styles.txCurrency}>{item.currency || 'ISB'}</Text>
      </View>
    </TouchableOpacity>
  );

  return (
    <ScrollView style={styles.container}>
      <Surface style={styles.balanceCard}>
        <Text style={styles.label}>Available Balance</Text>
        <Text style={styles.balance}>ISB {parseFloat(balance).toLocaleString()}</Text>
        <View style={styles.actions}>
          <Button
            mode="contained"
            onPress={() => navigation.navigate('Send')}
            icon="send"
            buttonColor="#00FF41"
            textColor="#000"
            style={styles.actionBtn}
          >
            Send
          </Button>
          <Button
            mode="outlined"
            onPress={() => navigation.navigate('Receive')}
            icon="download"
            textColor="#00FF41"
            style={[styles.actionBtn, { borderColor: '#00FF41' }]}
          >
            Receive
          </Button>
        </View>
      </Surface>

      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Recent Transactions</Text>
          <TouchableOpacity onPress={() => navigation.navigate('Transactions')}>
            <Text style={styles.viewAll}>View All</Text>
          </TouchableOpacity>
        </View>
        
        {transactions.length === 0 ? (
          <Surface style={styles.emptyState}>
            <Icon name="history" size={48} color="#333" />
            <Text style={styles.emptyText}>No transactions yet</Text>
          </Surface>
        ) : (
          transactions.map((tx, index) => (
            <React.Fragment key={tx.id || index}>
              {renderTransaction({ item: tx })}
              {index < transactions.length - 1 && <Divider style={styles.divider} />}
            </React.Fragment>
          ))
        )}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  balanceCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 24, marginBottom: 20 },
  label: { color: '#888', fontSize: 14 },
  balance: { color: '#00FF41', fontSize: 36, fontWeight: 'bold', marginVertical: 10 },
  actions: { flexDirection: 'row', justifyContent: 'space-between', marginTop: 15 },
  actionBtn: { flex: 1, marginHorizontal: 5 },
  section: { marginBottom: 20 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold' },
  viewAll: { color: '#00FF41', fontSize: 14 },
  txItem: { flexDirection: 'row', alignItems: 'center', paddingVertical: 12 },
  txIcon: { width: 44, height: 44, borderRadius: 22, justifyContent: 'center', alignItems: 'center' },
  txInfo: { flex: 1, marginLeft: 12 },
  txTitle: { color: '#fff', fontSize: 16, fontWeight: '500' },
  txAddress: { color: '#888', fontSize: 12, marginTop: 2 },
  txAmount: { alignItems: 'flex-end' },
  txValue: { fontSize: 16, fontWeight: 'bold' },
  txCurrency: { color: '#888', fontSize: 12 },
  divider: { backgroundColor: '#222', marginVertical: 4 },
  emptyState: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 40, alignItems: 'center' },
  emptyText: { color: '#666', marginTop: 15 },
});
