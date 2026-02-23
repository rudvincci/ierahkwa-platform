/**
 * Trade Screen - Swap & Exchange
 */

import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  TextInput,
} from 'react-native';
import { useTranslation } from 'react-i18next';

export default function TradeScreen() {
  const { t } = useTranslation();
  const [fromAmount, setFromAmount] = useState('');
  const [toAmount, setToAmount] = useState('');
  const [fromToken, setFromToken] = useState({ symbol: 'IGT-MAIN', logo: '🏛️' });
  const [toToken, setToToken] = useState({ symbol: 'USDT', logo: '💵' });

  const calculateSwap = (amount) => {
    setFromAmount(amount);
    // Mock exchange rate
    const rate = 1.0;
    setToAmount((parseFloat(amount || 0) * rate).toFixed(2));
  };

  const swapTokens = () => {
    const temp = fromToken;
    setFromToken(toToken);
    setToToken(temp);
    setFromAmount(toAmount);
    setToAmount(fromAmount);
  };

  return (
    <ScrollView style={styles.container}>
      {/* Swap Card */}
      <View style={styles.swapCard}>
        <Text style={styles.cardTitle}>💱 {t('swap')}</Text>
        
        {/* From Token */}
        <View style={styles.tokenInput}>
          <Text style={styles.inputLabel}>From</Text>
          <View style={styles.inputRow}>
            <TextInput
              style={styles.amountInput}
              placeholder="0.0"
              placeholderTextColor="#666"
              keyboardType="numeric"
              value={fromAmount}
              onChangeText={calculateSwap}
            />
            <TouchableOpacity style={styles.tokenSelect}>
              <Text style={styles.tokenLogo}>{fromToken.logo}</Text>
              <Text style={styles.tokenSymbol}>{fromToken.symbol}</Text>
              <Text style={styles.chevron}>▼</Text>
            </TouchableOpacity>
          </View>
          <Text style={styles.balanceText}>Balance: 100,000</Text>
        </View>

        {/* Swap Button */}
        <TouchableOpacity style={styles.swapButton} onPress={swapTokens}>
          <Text style={styles.swapIcon}>🔄</Text>
        </TouchableOpacity>

        {/* To Token */}
        <View style={styles.tokenInput}>
          <Text style={styles.inputLabel}>To</Text>
          <View style={styles.inputRow}>
            <TextInput
              style={styles.amountInput}
              placeholder="0.0"
              placeholderTextColor="#666"
              keyboardType="numeric"
              value={toAmount}
              editable={false}
            />
            <TouchableOpacity style={styles.tokenSelect}>
              <Text style={styles.tokenLogo}>{toToken.logo}</Text>
              <Text style={styles.tokenSymbol}>{toToken.symbol}</Text>
              <Text style={styles.chevron}>▼</Text>
            </TouchableOpacity>
          </View>
          <Text style={styles.balanceText}>Balance: 5,000</Text>
        </View>

        {/* Swap Info */}
        <View style={styles.swapInfo}>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>Rate</Text>
            <Text style={styles.infoValue}>1 IGT = 1.00 USDT</Text>
          </View>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>Fee</Text>
            <Text style={styles.infoValue}>0.1%</Text>
          </View>
          <View style={styles.infoRow}>
            <Text style={styles.infoLabel}>Slippage</Text>
            <Text style={styles.infoValue}>0.5%</Text>
          </View>
        </View>

        {/* Execute Button */}
        <TouchableOpacity style={styles.executeBtn}>
          <Text style={styles.executeBtnText}>{t('swap')}</Text>
        </TouchableOpacity>
      </View>

      {/* Popular Pairs */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Popular Pairs</Text>
        <View style={styles.pairsGrid}>
          <PairCard from="IGT" to="USDT" rate="1.00" change="+2.5%" />
          <PairCard from="ETH" to="IGT" rate="2500" change="-0.8%" />
          <PairCard from="BTC" to="IGT" rate="45000" change="+1.2%" />
          <PairCard from="BNB" to="IGT" rate="300" change="+0.5%" />
        </View>
      </View>

      {/* Quick Amounts */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Quick Amounts</Text>
        <View style={styles.quickAmounts}>
          {['25%', '50%', '75%', 'MAX'].map((pct) => (
            <TouchableOpacity
              key={pct}
              style={styles.quickBtn}
              onPress={() => calculateSwap(pct === 'MAX' ? '100000' : String(100000 * parseInt(pct) / 100))}
            >
              <Text style={styles.quickBtnText}>{pct}</Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>
    </ScrollView>
  );
}

function PairCard({ from, to, rate, change }) {
  const isPositive = change.startsWith('+');
  return (
    <View style={styles.pairCard}>
      <Text style={styles.pairName}>{from}/{to}</Text>
      <Text style={styles.pairRate}>{rate}</Text>
      <Text style={[styles.pairChange, { color: isPositive ? '#00FF41' : '#FF6B35' }]}>
        {change}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0a0e17',
  },
  swapCard: {
    margin: 15,
    padding: 20,
    backgroundColor: '#1a1f2e',
    borderRadius: 20,
    borderWidth: 1,
    borderColor: '#9D4EDD',
  },
  cardTitle: {
    color: '#fff',
    fontSize: 20,
    fontWeight: 'bold',
    marginBottom: 20,
    textAlign: 'center',
  },
  tokenInput: {
    backgroundColor: 'rgba(0,0,0,0.3)',
    borderRadius: 15,
    padding: 15,
    marginBottom: 10,
  },
  inputLabel: {
    color: '#888',
    marginBottom: 10,
  },
  inputRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  amountInput: {
    flex: 1,
    fontSize: 28,
    color: '#fff',
    fontWeight: 'bold',
  },
  tokenSelect: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(157,78,221,0.2)',
    paddingHorizontal: 15,
    paddingVertical: 10,
    borderRadius: 12,
    gap: 8,
  },
  tokenLogo: {
    fontSize: 20,
  },
  tokenSymbol: {
    color: '#fff',
    fontWeight: 'bold',
  },
  chevron: {
    color: '#888',
    fontSize: 10,
  },
  balanceText: {
    color: '#666',
    marginTop: 10,
    fontSize: 12,
  },
  swapButton: {
    alignSelf: 'center',
    backgroundColor: '#9D4EDD',
    width: 50,
    height: 50,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
    marginVertical: 10,
  },
  swapIcon: {
    fontSize: 24,
  },
  swapInfo: {
    backgroundColor: 'rgba(0,255,255,0.1)',
    borderRadius: 12,
    padding: 15,
    marginVertical: 15,
  },
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 5,
  },
  infoLabel: {
    color: '#888',
  },
  infoValue: {
    color: '#00FFFF',
  },
  executeBtn: {
    backgroundColor: '#9D4EDD',
    borderRadius: 15,
    padding: 18,
    alignItems: 'center',
  },
  executeBtnText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
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
  pairsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 10,
  },
  pairCard: {
    width: '48%',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    alignItems: 'center',
  },
  pairName: {
    color: '#fff',
    fontWeight: 'bold',
  },
  pairRate: {
    color: '#FFD700',
    fontSize: 18,
    fontWeight: 'bold',
    marginVertical: 5,
  },
  pairChange: {
    fontSize: 12,
  },
  quickAmounts: {
    flexDirection: 'row',
    gap: 10,
  },
  quickBtn: {
    flex: 1,
    backgroundColor: '#1a1f2e',
    borderRadius: 10,
    padding: 12,
    alignItems: 'center',
  },
  quickBtnText: {
    color: '#00FFFF',
    fontWeight: 'bold',
  },
});
