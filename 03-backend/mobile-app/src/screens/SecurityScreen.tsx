/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  SECURITY SCREEN - AI Agents Status & Cyber Defense
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Displays real-time status of sovereign AI security agents,
 *   cyber defense metrics, threat alerts, and system integrity monitoring.
 *   Integrates with the NEXUS Escudo (Defense) system.
 *
 * @module screens/SecurityScreen
 * @requires react-native
 * @requires react-native-paper
 * @requires react-i18next
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  RefreshControl,
} from 'react-native';
import { Text, Surface, Button, ProgressBar } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';

/**
 * SecurityScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The AI agents and security status screen
 */
export default function SecurityScreen({ navigation }) {
  const { t } = useTranslation();
  const [refreshing, setRefreshing] = useState(false);

  const [systemStatus] = useState({
    overallThreatLevel: 'LOW',
    firewallStatus: 'ACTIVE',
    encryptionStatus: 'QUANTUM-SECURED',
    lastScan: '2 minutes ago',
    blockedThreats24h: 1247,
    activeAgents: 18,
    totalAgents: 20,
  });

  const [aiAgents] = useState([
    { id: 'sentinel-01', name: 'Sentinel Alpha', type: 'Perimeter Defense', status: 'active', cpu: 0.23, threats: 45, uptime: '99.99%', color: '#00FF41' },
    { id: 'guardian-01', name: 'Guardian Shield', type: 'Firewall AI', status: 'active', cpu: 0.45, threats: 312, uptime: '99.97%', color: '#00FFFF' },
    { id: 'eagle-01', name: 'Eagle Eye', type: 'Anomaly Detection', status: 'active', cpu: 0.67, threats: 89, uptime: '99.95%', color: '#FFD700' },
    { id: 'thunder-01', name: 'Thunder Response', type: 'Incident Response', status: 'active', cpu: 0.12, threats: 23, uptime: '100%', color: '#E040FB' },
    { id: 'crypto-01', name: 'Crypto Warden', type: 'Encryption Monitor', status: 'active', cpu: 0.34, threats: 67, uptime: '99.98%', color: '#7C4DFF' },
    { id: 'hawk-01', name: 'Night Hawk', type: 'Network Scanner', status: 'standby', cpu: 0.05, threats: 0, uptime: '99.90%', color: '#FF9100' },
  ]);

  const [recentAlerts] = useState([
    { id: '1', type: 'warning', message: 'Unusual login pattern detected from IP 45.33.x.x', time: '5 min ago', severity: 'medium' },
    { id: '2', type: 'blocked', message: 'DDoS attempt blocked - 2.3M requests neutralized', time: '23 min ago', severity: 'high' },
    { id: '3', type: 'info', message: 'Quantum key rotation completed successfully', time: '1 hour ago', severity: 'low' },
    { id: '4', type: 'blocked', message: 'SQL injection attempt on API gateway', time: '2 hours ago', severity: 'high' },
  ]);

  const onRefresh = async () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 1500);
  };

  /**
   * Returns the appropriate color for a given threat severity
   * @param {string} severity - low, medium, or high
   * @returns {string} Hex color code
   */
  const getSeverityColor = (severity) => {
    const colors = { low: '#00FF41', medium: '#FFD700', high: '#FF4444' };
    return colors[severity] || '#888';
  };

  /**
   * Returns the appropriate icon for a given alert type
   * @param {string} type - warning, blocked, or info
   * @returns {string} Icon name
   */
  const getAlertIcon = (type) => {
    const icons = { warning: 'alert-circle', blocked: 'shield-off', info: 'information' };
    return icons[type] || 'alert';
  };

  return (
    <ScrollView
      style={styles.container}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      accessibilityLabel={t('sec_screen_title') || 'Security Dashboard Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>{t('sec_title') || 'Security Center'}</Text>
        <Text style={styles.headerSubtitle}>{t('sec_subtitle') || 'NEXUS Escudo - AI Defense Network'}</Text>
      </View>

      {/* Overall Status */}
      <Surface style={styles.statusCard} accessibilityLabel={`Overall threat level: ${systemStatus.overallThreatLevel}`}>
        <View style={styles.statusHeader}>
          <View style={[styles.statusIndicator, { backgroundColor: '#00FF41' }]} />
          <Text style={styles.statusTitle}>{t('sec_system_status') || 'System Status'}</Text>
        </View>
        <View style={styles.statusGrid}>
          <View style={styles.statusItem}>
            <Icon name="shield-check" size={32} color="#00FF41" />
            <Text style={styles.statusValue}>{systemStatus.overallThreatLevel}</Text>
            <Text style={styles.statusLabel}>{t('sec_threat_level') || 'Threat Level'}</Text>
          </View>
          <View style={styles.statusItem}>
            <Icon name="fire-alert" size={32} color="#00FFFF" />
            <Text style={styles.statusValue}>{systemStatus.firewallStatus}</Text>
            <Text style={styles.statusLabel}>{t('sec_firewall') || 'Firewall'}</Text>
          </View>
          <View style={styles.statusItem}>
            <Icon name="lock" size={32} color="#FFD700" />
            <Text style={[styles.statusValue, { fontSize: 12 }]}>{systemStatus.encryptionStatus}</Text>
            <Text style={styles.statusLabel}>{t('sec_encryption') || 'Encryption'}</Text>
          </View>
        </View>
        <View style={styles.statusFooter}>
          <Text style={styles.statusFooterText}>
            {t('sec_blocked_today') || 'Blocked today'}: <Text style={{ color: '#FF4444' }}>{systemStatus.blockedThreats24h.toLocaleString()}</Text>
          </Text>
          <Text style={styles.statusFooterText}>
            {t('sec_last_scan') || 'Last scan'}: {systemStatus.lastScan}
          </Text>
        </View>
      </Surface>

      {/* AI Agents */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('sec_ai_agents') || 'AI Security Agents'}</Text>
          <Text style={styles.agentCount}>
            <Text style={{ color: '#00FF41' }}>{systemStatus.activeAgents}</Text>/{systemStatus.totalAgents} {t('sec_online') || 'Online'}
          </Text>
        </View>
        {aiAgents.map((agent) => (
          <TouchableOpacity
            key={agent.id}
            onPress={() => navigation.navigate('AgentDetail', { agentId: agent.id })}
            accessibilityLabel={`${agent.name}, ${agent.type}, status: ${agent.status}`}
            accessibilityRole="button"
          >
            <Surface style={styles.agentCard}>
              <View style={styles.agentHeader}>
                <View style={[styles.agentStatus, { backgroundColor: agent.status === 'active' ? '#00FF41' : '#FFD700' }]} />
                <View style={styles.agentInfo}>
                  <Text style={styles.agentName}>{agent.name}</Text>
                  <Text style={styles.agentType}>{agent.type}</Text>
                </View>
                <Text style={[styles.agentStatusText, { color: agent.status === 'active' ? '#00FF41' : '#FFD700' }]}>
                  {agent.status.toUpperCase()}
                </Text>
              </View>
              <View style={styles.agentMetrics}>
                <View style={styles.agentMetric}>
                  <Text style={styles.metricLabel}>CPU</Text>
                  <ProgressBar progress={agent.cpu} color={agent.color} style={styles.metricBar} />
                  <Text style={styles.metricValue}>{Math.round(agent.cpu * 100)}%</Text>
                </View>
                <View style={styles.agentMetricRow}>
                  <Text style={styles.metricLabel}>{t('sec_threats') || 'Threats'}: <Text style={{ color: '#FF4444' }}>{agent.threats}</Text></Text>
                  <Text style={styles.metricLabel}>{t('sec_uptime') || 'Uptime'}: <Text style={{ color: '#00FF41' }}>{agent.uptime}</Text></Text>
                </View>
              </View>
            </Surface>
          </TouchableOpacity>
        ))}
      </View>

      {/* Recent Alerts */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('sec_alerts') || 'Recent Alerts'}</Text>
          <TouchableOpacity onPress={() => navigation.navigate('AllAlerts')}>
            <Text style={styles.viewAll}>{t('view_all') || 'View All'}</Text>
          </TouchableOpacity>
        </View>
        {recentAlerts.map((alert) => (
          <Surface key={alert.id} style={styles.alertCard}>
            <Icon name={getAlertIcon(alert.type)} size={20} color={getSeverityColor(alert.severity)} />
            <View style={styles.alertInfo}>
              <Text style={styles.alertMessage}>{alert.message}</Text>
              <Text style={styles.alertTime}>{alert.time}</Text>
            </View>
            <View style={[styles.severityBadge, { backgroundColor: getSeverityColor(alert.severity) + '20' }]}>
              <Text style={[styles.severityText, { color: getSeverityColor(alert.severity) }]}>
                {alert.severity.toUpperCase()}
              </Text>
            </View>
          </Surface>
        ))}
      </View>

      {/* Quick Actions */}
      <View style={styles.quickActions}>
        <Button
          mode="contained"
          onPress={() => navigation.navigate('RunScan')}
          icon="radar"
          buttonColor="#00FF41"
          textColor="#000"
          style={styles.quickBtn}
          accessibilityLabel="Run full security scan"
        >
          {t('sec_scan') || 'Full Scan'}
        </Button>
        <Button
          mode="outlined"
          onPress={() => navigation.navigate('SecurityReport')}
          icon="file-chart"
          textColor="#00FF41"
          style={[styles.quickBtn, { borderColor: '#00FF41' }]}
          accessibilityLabel="View security report"
        >
          {t('sec_report') || 'Report'}
        </Button>
      </View>

      <View style={{ height: 30 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#f44336', fontSize: 14, marginTop: 4 },
  statusCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 20, marginBottom: 20, borderWidth: 1, borderColor: '#00FF4140' },
  statusHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 15 },
  statusIndicator: { width: 12, height: 12, borderRadius: 6 },
  statusTitle: { color: '#fff', fontSize: 16, fontWeight: 'bold', marginLeft: 10 },
  statusGrid: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 15 },
  statusItem: { alignItems: 'center', flex: 1 },
  statusValue: { color: '#fff', fontSize: 14, fontWeight: 'bold', marginTop: 6 },
  statusLabel: { color: '#888', fontSize: 11, marginTop: 4 },
  statusFooter: { flexDirection: 'row', justifyContent: 'space-between', borderTopWidth: 1, borderTopColor: '#333', paddingTop: 12 },
  statusFooterText: { color: '#888', fontSize: 12 },
  section: { marginBottom: 25 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 0 },
  agentCount: { color: '#888', fontSize: 14 },
  viewAll: { color: '#00FF41', fontSize: 14 },
  agentCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, marginBottom: 10 },
  agentHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 10 },
  agentStatus: { width: 10, height: 10, borderRadius: 5 },
  agentInfo: { flex: 1, marginLeft: 12 },
  agentName: { color: '#fff', fontSize: 16, fontWeight: '600' },
  agentType: { color: '#888', fontSize: 12, marginTop: 2 },
  agentStatusText: { fontSize: 11, fontWeight: 'bold' },
  agentMetrics: {},
  agentMetric: { marginBottom: 6 },
  metricLabel: { color: '#888', fontSize: 12 },
  metricBar: { height: 4, borderRadius: 2, backgroundColor: '#333', marginVertical: 4 },
  metricValue: { color: '#fff', fontSize: 12 },
  agentMetricRow: { flexDirection: 'row', justifyContent: 'space-between' },
  alertCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 14, flexDirection: 'row', alignItems: 'center', marginBottom: 8 },
  alertInfo: { flex: 1, marginLeft: 12 },
  alertMessage: { color: '#fff', fontSize: 13 },
  alertTime: { color: '#888', fontSize: 11, marginTop: 4 },
  severityBadge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 12 },
  severityText: { fontSize: 10, fontWeight: 'bold' },
  quickActions: { flexDirection: 'row', justifyContent: 'space-between' },
  quickBtn: { flex: 1, marginHorizontal: 5, borderRadius: 12 },
});
