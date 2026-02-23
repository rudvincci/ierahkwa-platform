/**
 * Bridge Screen - Cross-Chain Transfers
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
  Alert,
} from 'react-native';
import api from '../services/api';

export default function BridgeScreen() {
  const [chains, setChains] = useState([]);
  const [tokens, setTokens] = useState({});
  const [fromChain, setFromChain] = useState({ id: 1, name: 'Ethereum', icon: 'Ξ' });
  const [toChain, setToChain] = useState({ id: 777777, name: 'Ierahkwa', icon: '🏛️' });
  const [selectedToken, setSelectedToken] = useState('WETH');
  const [amount, setAmount] = useState('');
  const [history, setHistory] = useState([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [chainsData, tokensData, historyData] = await Promise.all([
        api.getBridgeChains(),
        api.getBridgeTokens(),
        api.getBridgeHistory(),
      ]);
      setChains(chainsData.chains || []);
      setTokens(tokensData.tokens || {});
      setHistory([...historyData.pending, ...historyData.completed]);
    } catch (e) {
      console.error('Error:', e);
    }
  };

  const swapChains = () => {
    const temp = fromChain;
    setFromChain(toChain);
    setToChain(temp);
  };

  const initiateBridge = async () => {
    if (!amount || parseFloat(amount) <= 0) {
      Alert.alert('Error', 'Please enter an amount');
      return;
    }

    try {
      const result = await api.initiateBridgeDeposit({
        fromChain: fromChain.id,
        toChain: toChain.id,
        token: selectedToken,
        amount,
        fromAddress: '0x' + '1'.repeat(40),
        toAddress: '0x' + '2'.repeat(40),
      });

      Alert.alert(
        '🌉 Bridge Initiated',
        `ID: ${result.bridge.id}\nStatus: ${result.bridge.status}\nEstimated: ~10 seconds`
      );
      setAmount('');
      loadData();
    } catch (e) {
      Alert.alert('Error', e.message);
    }
  };

  const fee = (parseFloat(amount || 0) * 0.001).toFixed(6);
  const receive = (parseFloat(amount || 0) - parseFloat(fee)).toFixed(6);

  return (
    <ScrollView style={styles.container}>
      {/* Bridge Card */}
      <View style={styles.bridgeCard}>
        <Text style={styles.cardTitle}>🌉 Cross-Chain Bridge</Text>

        {/* From Chain */}
        <View style={styles.chainBox}>
          <Text style={styles.chainLabel}>From</Text>
          <View style={styles.chainInfo}>
            <Text style={styles.chainIcon}>{fromChain.icon}</Text>
            <View>
              <Text style={styles.chainName}>{fromChain.name}</Text>
              <Text style={styles.chainId}>Chain ID: {fromChain.id}</Text>
            </View>
          </View>
        </View>

        {/* Swap Button */}
        <TouchableOpacity style={styles.swapBtn} onPress={swapChains}>
          <Text style={styles.swapIcon}>🔄</Text>
        </TouchableOpacity>

        {/* To Chain */}
        <View style={[styles.chainBox, styles.chainBoxActive]}>
          <Text style={styles.chainLabel}>To</Text>
          <View style={styles.chainInfo}>
            <Text style={styles.chainIcon}>{toChain.icon}</Text>
            <View>
              <Text style={styles.chainName}>{toChain.name}</Text>
              <Text style={styles.chainId}>Chain ID: {toChain.id}</Text>
            </View>
          </View>
        </View>

        {/* Token & Amount */}
        <View style={styles.inputSection}>
          <Text style={styles.inputLabel}>Amount</Text>
          <View style={styles.inputRow}>
            <TextInput
              style={styles.amountInput}
              placeholder="0.0"
              placeholderTextColor="#666"
              keyboardType="numeric"
              value={amount}
              onChangeText={setAmount}
            />
            <TouchableOpacity style={styles.tokenSelect}>
              <Text style={styles.tokenLogo}>{tokens[selectedToken]?.logo || '🪙'}</Text>
              <Text style={styles.tokenSymbol}>{selectedToken}</Text>
            </TouchableOpacity>
          </View>
        </View>

        {/* Info */}
        <View style={styles.infoBox}>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>Bridge Fee</Text>
            <Text style={styles.infoValue}>0.1%</Text>
          </View>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>Estimated Time</Text>
            <Text style={styles.infoValue}>~10 seconds</Text>
          </View>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>You Receive</Text>
            <Text style={[styles.infoValue, { color: '#00FF41' }]}>
              {receive} {selectedToken}
            </Text>
          </View>
        </View>

        {/* Bridge Button */}
        <TouchableOpacity style={styles.bridgeBtn} onPress={initiateBridge}>
          <Text style={styles.bridgeBtnText}>🌉 Bridge Tokens</Text>
        </TouchableOpacity>
      </View>

      {/* Supported Tokens */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Supported Tokens</Text>
        <ScrollView horizontal showsHorizontalScrollIndicator={false}>
          {Object.entries(tokens).map(([symbol, token]) => (
            <TouchableOpacity
              key={symbol}
              style={[styles.tokenCard, selectedToken === symbol && styles.tokenCardActive]}
              onPress={() => setSelectedToken(symbol)}
            >
              <Text style={styles.tokenCardIcon}>{token.logo}</Text>
              <Text style={styles.tokenCardSymbol}>{symbol}</Text>
            </TouchableOpacity>
          ))}
        </ScrollView>
      </View>

      {/* History */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Recent Bridges</Text>
        {history.length === 0 ? (
          <Text style={styles.emptyText}>No bridge history yet</Text>
        ) : (
          history.slice(0, 5).map((b, i) => (
            <View key={i} style={styles.historyItem}>
              <View>
                <Text style={styles.historyAmount}>{b.amount} {b.token}</Text>
                <Text style={styles.historyChains}>
                  Chain {b.fromChain} → Chain {b.toChain}
                </Text>
              </View>
              <Text style={[styles.historyStatus, { color: b.status === 'COMPLETED' ? '#00FF41' : '#FFD700' }]}>
                {b.status}
              </Text>
            </View>
          ))
        )}
      </View>

      {/* Supported Chains */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Supported Chains</Text>
        <View style={styles.chainsGrid}>
          {chains.map(chain => (
            <TouchableOpacity
              key={chain.id}
              style={styles.chainCard}
              onPress={() => setFromChain({ ...chain, icon: getChainIcon(chain.symbol) })}
            >
              <Text style={styles.chainCardIcon}>{getChainIcon(chain.symbol)}</Text>
              <Text style={styles.chainCardName}>{chain.name}</Text>
              <Text style={[styles.chainCardStatus, { color: chain.status === 'ACTIVE' ? '#00FF41' : '#FF6666' }]}>
                ● {chain.status}
              </Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>
    </ScrollView>
  );
}

function getChainIcon(symbol) {
  const icons = { ETH: 'Ξ', BNB: '🟡', MATIC: '🟣', AVAX: '🔺', ARB: '🔵', OP: '🔴' };
  return icons[symbol] || '⛓️';
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0a0e17' },
  bridgeCard: {
    margin: 15,
    padding: 20,
    backgroundColor: '#1a1f2e',
    borderRadius: 20,
    borderWidth: 2,
    borderColor: '#9D4EDD',
  },
  cardTitle: { color: '#fff', fontSize: 20, fontWeight: 'bold', textAlign: 'center', marginBottom: 20 },
  chainBox: {
    backgroundColor: 'rgba(0,0,0,0.3)',
    borderRadius: 15,
    padding: 15,
    marginBottom: 10,
  },
  chainBoxActive: { borderWidth: 1, borderColor: '#00FF41' },
  chainLabel: { color: '#888', marginBottom: 10 },
  chainInfo: { flexDirection: 'row', alignItems: 'center', gap: 15 },
  chainIcon: { fontSize: 32 },
  chainName: { color: '#fff', fontSize: 18, fontWeight: 'bold' },
  chainId: { color: '#888', fontSize: 12 },
  swapBtn: {
    alignSelf: 'center',
    backgroundColor: '#9D4EDD',
    width: 50,
    height: 50,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
    marginVertical: 10,
  },
  swapIcon: { fontSize: 24 },
  inputSection: { marginTop: 15 },
  inputLabel: { color: '#888', marginBottom: 10 },
  inputRow: { flexDirection: 'row', alignItems: 'center', gap: 10 },
  amountInput: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.3)',
    borderRadius: 12,
    padding: 15,
    color: '#fff',
    fontSize: 24,
    fontWeight: 'bold',
  },
  tokenSelect: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(157,78,221,0.2)',
    paddingHorizontal: 15,
    paddingVertical: 12,
    borderRadius: 12,
    gap: 8,
  },
  tokenLogo: { fontSize: 20 },
  tokenSymbol: { color: '#fff', fontWeight: 'bold' },
  infoBox: {
    backgroundColor: 'rgba(0,255,255,0.1)',
    borderRadius: 12,
    padding: 15,
    marginVertical: 15,
  },
  infoRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 5 },
  infoLabel: { color: '#888' },
  infoValue: { color: '#00FFFF' },
  bridgeBtn: {
    backgroundColor: '#9D4EDD',
    borderRadius: 15,
    padding: 18,
    alignItems: 'center',
  },
  bridgeBtnText: { color: '#fff', fontSize: 18, fontWeight: 'bold' },
  section: { padding: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  tokenCard: {
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    marginRight: 10,
    alignItems: 'center',
    minWidth: 80,
  },
  tokenCardActive: { borderWidth: 1, borderColor: '#9D4EDD' },
  tokenCardIcon: { fontSize: 28, marginBottom: 5 },
  tokenCardSymbol: { color: '#fff', fontWeight: 'bold' },
  chainsGrid: { flexDirection: 'row', flexWrap: 'wrap', gap: 10 },
  chainCard: {
    width: '31%',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    alignItems: 'center',
  },
  chainCardIcon: { fontSize: 24, marginBottom: 5 },
  chainCardName: { color: '#fff', fontWeight: 'bold', fontSize: 12 },
  chainCardStatus: { fontSize: 10, marginTop: 5 },
  historyItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    marginBottom: 10,
  },
  historyAmount: { color: '#fff', fontWeight: 'bold' },
  historyChains: { color: '#888', fontSize: 12 },
  historyStatus: { fontWeight: 'bold' },
  emptyText: { color: '#666', textAlign: 'center' },
});
