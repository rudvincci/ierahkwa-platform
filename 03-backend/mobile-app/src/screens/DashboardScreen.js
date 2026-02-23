/**
 * Dashboard Screen - Main Hub
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { useTranslation } from 'react-i18next';
import api from '../services/api';

export default function DashboardScreen({ navigation }) {
  const { t } = useTranslation();
  const [stats, setStats] = useState(null);
  const [tokens, setTokens] = useState([]);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [statsRes, tokensRes] = await Promise.all([
        api.getStats(),
        api.getTokens(),
      ]);
      setStats(statsRes);
      setTokens(tokensRes.slice(0, 10));
    } catch (e) {
      console.error('Error loading data:', e);
    }
  };

  const onRefresh = async () => {
    setRefreshing(true);
    await loadData();
    setRefreshing(false);
  };

  return (
    <ScrollView
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
      }
    >
      {/* Welcome Banner */}
      <View style={styles.banner}>
        <Text style={styles.bannerIcon}>🏛️</Text>
        <Text style={styles.bannerTitle}>{t('welcome')}</Text>
        <Text style={styles.bannerSubtitle}>Sovereign Digital Ecosystem</Text>
      </View>

      {/* Quick Stats */}
      <View style={styles.statsRow}>
        <StatCard
          icon="⛓️"
          value={stats?.blockNumber || 0}
          label="Blocks"
          color="#00FF41"
        />
        <StatCard
          icon="🪙"
          value={stats?.totalTokens || 103}
          label="Tokens"
          color="#FFD700"
        />
        <StatCard
          icon="💱"
          value={stats?.totalTransactions || 0}
          label="TX"
          color="#00FFFF"
        />
      </View>

      {/* Quick Actions */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Quick Actions</Text>
        <View style={styles.actionsGrid}>
          <ActionButton
            icon="💱"
            label={t('swap')}
            color="#9D4EDD"
            onPress={() => navigation.navigate('Trade')}
          />
          <ActionButton
            icon="🌉"
            label={t('bridge')}
            color="#FF6B35"
            onPress={() => navigation.navigate('Bridge')}
          />
          <ActionButton
            icon="🗳️"
            label={t('voting')}
            color="#FFD700"
            onPress={() => navigation.navigate('Governance')}
          />
          <ActionButton
            icon="🏆"
            label={t('rewards')}
            color="#00FF41"
            onPress={() => navigation.navigate('Rewards')}
          />
        </View>
      </View>

      {/* Top Tokens */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>🪙 Top Tokens</Text>
        {tokens.map((token, i) => (
          <TouchableOpacity
            key={token.symbol}
            style={styles.tokenItem}
            onPress={() => navigation.navigate('TokenDetail', { token })}
          >
            <View style={styles.tokenInfo}>
              <Text style={styles.tokenLogo}>{token.logo || '🪙'}</Text>
              <View>
                <Text style={styles.tokenName}>{token.name}</Text>
                <Text style={styles.tokenSymbol}>{token.symbol}</Text>
              </View>
            </View>
            <View style={styles.tokenPrice}>
              <Text style={styles.priceText}>
                {Number(token.totalSupply).toLocaleString()}
              </Text>
              <Text style={styles.statusText}>● {token.status}</Text>
            </View>
          </TouchableOpacity>
        ))}
      </View>

      {/* Network Status */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>🌐 Network Status</Text>
        <View style={styles.networkCard}>
          <View style={styles.networkRow}>
            <Text style={styles.networkLabel}>Chain ID</Text>
            <Text style={styles.networkValue}>777777</Text>
          </View>
          <View style={styles.networkRow}>
            <Text style={styles.networkLabel}>Block Time</Text>
            <Text style={styles.networkValue}>500ms</Text>
          </View>
          <View style={styles.networkRow}>
            <Text style={styles.networkLabel}>Gas Price</Text>
            <Text style={[styles.networkValue, { color: '#00FF41' }]}>FREE</Text>
          </View>
          <View style={styles.networkRow}>
            <Text style={styles.networkLabel}>Status</Text>
            <Text style={[styles.networkValue, { color: '#00FF41' }]}>● LIVE</Text>
          </View>
        </View>
      </View>
    </ScrollView>
  );
}

// Stat Card Component
function StatCard({ icon, value, label, color }) {
  return (
    <View style={styles.statCard}>
      <Text style={styles.statIcon}>{icon}</Text>
      <Text style={[styles.statValue, { color }]}>{value.toLocaleString()}</Text>
      <Text style={styles.statLabel}>{label}</Text>
    </View>
  );
}

// Action Button Component
function ActionButton({ icon, label, color, onPress }) {
  return (
    <TouchableOpacity
      style={[styles.actionBtn, { borderColor: color }]}
      onPress={onPress}
    >
      <Text style={styles.actionIcon}>{icon}</Text>
      <Text style={styles.actionLabel}>{label}</Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0a0e17',
  },
  banner: {
    padding: 30,
    alignItems: 'center',
    borderBottomWidth: 1,
    borderBottomColor: '#1a1f2e',
  },
  bannerIcon: {
    fontSize: 50,
    marginBottom: 10,
  },
  bannerTitle: {
    color: '#FFD700',
    fontSize: 20,
    fontWeight: 'bold',
  },
  bannerSubtitle: {
    color: '#888',
    marginTop: 5,
  },
  statsRow: {
    flexDirection: 'row',
    padding: 15,
    gap: 10,
  },
  statCard: {
    flex: 1,
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    alignItems: 'center',
  },
  statIcon: {
    fontSize: 24,
    marginBottom: 5,
  },
  statValue: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  statLabel: {
    color: '#888',
    fontSize: 12,
    marginTop: 3,
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
  actionsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 10,
  },
  actionBtn: {
    width: '48%',
    backgroundColor: '#1a1f2e',
    borderWidth: 1,
    borderRadius: 12,
    padding: 20,
    alignItems: 'center',
  },
  actionIcon: {
    fontSize: 30,
    marginBottom: 8,
  },
  actionLabel: {
    color: '#fff',
    fontWeight: '600',
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
    fontSize: 28,
  },
  tokenName: {
    color: '#fff',
    fontWeight: '600',
  },
  tokenSymbol: {
    color: '#888',
    fontSize: 12,
  },
  tokenPrice: {
    alignItems: 'flex-end',
  },
  priceText: {
    color: '#FFD700',
    fontWeight: 'bold',
  },
  statusText: {
    color: '#00FF41',
    fontSize: 11,
  },
  networkCard: {
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
  },
  networkRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#2a2f3e',
  },
  networkLabel: {
    color: '#888',
  },
  networkValue: {
    color: '#fff',
    fontWeight: '600',
  },
});
