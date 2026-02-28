/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  GOVERNMENT SCREEN - Citizen Services Portal
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Provides access to sovereign citizen services including
 *   document management, permit applications, tax filing, census data,
 *   and direct communication with government agencies.
 *
 * @module screens/GovernmentScreen
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
  AccessibilityInfo,
} from 'react-native';
import { Text, Surface, Card, Button, Badge, Searchbar } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';

/** @typedef {Object} ServiceCategory */
const SERVICE_CATEGORIES = [
  { id: 'identity', icon: 'card-account-details', labelKey: 'gov_identity', color: '#FFD700', count: 3 },
  { id: 'permits', icon: 'file-certificate', labelKey: 'gov_permits', color: '#00FF41', count: 5 },
  { id: 'taxes', icon: 'calculator', labelKey: 'gov_taxes', color: '#00FFFF', count: 2 },
  { id: 'census', icon: 'account-group', labelKey: 'gov_census', color: '#E040FB', count: 1 },
  { id: 'legal', icon: 'scale-balance', labelKey: 'gov_legal', color: '#FF9100', count: 4 },
  { id: 'land', icon: 'map-marker-radius', labelKey: 'gov_land', color: '#43A047', count: 3 },
];

/** @typedef {Object} PendingRequest */

/**
 * GovernmentScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The government citizen services screen
 */
export default function GovernmentScreen({ navigation }) {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [refreshing, setRefreshing] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [pendingRequests, setPendingRequests] = useState([
    { id: '1', title: 'Citizen ID Renewal', status: 'processing', date: '2026-02-25' },
    { id: '2', title: 'Land Registry Update', status: 'pending', date: '2026-02-20' },
  ]);

  const onRefresh = async () => {
    setRefreshing(true);
    // Simulate API call
    setTimeout(() => setRefreshing(false), 1500);
  };

  /**
   * Returns the appropriate color for a given request status
   * @param {string} status - The status string
   * @returns {string} Hex color code
   */
  const getStatusColor = (status) => {
    const colors = {
      pending: '#FFD700',
      processing: '#00FFFF',
      approved: '#00FF41',
      rejected: '#FF4444',
    };
    return colors[status] || '#888';
  };

  return (
    <ScrollView
      style={styles.container}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      accessibilityLabel={t('gov_screen_title') || 'Government Services Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>{t('gov_citizen_services') || 'Citizen Services'}</Text>
        <Text style={styles.headerSubtitle}>
          {t('gov_subtitle') || 'Ierahkwa Ne Kanienke - Sovereign Government'}
        </Text>
      </View>

      {/* Citizen ID Card */}
      <Surface style={styles.citizenCard} accessibilityLabel="Citizen ID Card">
        <View style={styles.citizenCardHeader}>
          <Icon name="shield-star" size={28} color="#FFD700" />
          <Text style={styles.citizenCardTitle}>{t('gov_citizen_id') || 'Sovereign Citizen ID'}</Text>
        </View>
        <View style={styles.citizenCardBody}>
          <View>
            <Text style={styles.citizenName}>
              {user?.firstName} {user?.lastName}
            </Text>
            <Text style={styles.citizenIdNumber}>{user?.citizenId || 'IK-2026-000001'}</Text>
            <Text style={styles.citizenStatus}>
              <Icon name="check-circle" size={14} color="#00FF41" /> {t('gov_verified') || 'Verified Citizen'}
            </Text>
          </View>
          <TouchableOpacity
            style={styles.qrButton}
            onPress={() => navigation.navigate('CitizenQR')}
            accessibilityLabel="Show QR code for citizen ID"
            accessibilityRole="button"
          >
            <Icon name="qrcode" size={48} color="#00FF41" />
          </TouchableOpacity>
        </View>
      </Surface>

      {/* Search */}
      <Searchbar
        placeholder={t('gov_search') || 'Search government services...'}
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
        inputStyle={{ color: '#fff' }}
        iconColor="#888"
        placeholderTextColor="#666"
        accessibilityLabel="Search government services"
      />

      {/* Service Categories */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('gov_services') || 'Services'}</Text>
        <View style={styles.categoriesGrid}>
          {SERVICE_CATEGORIES.map((category) => (
            <TouchableOpacity
              key={category.id}
              style={styles.categoryCard}
              onPress={() => navigation.navigate('GovService', { serviceId: category.id })}
              accessibilityLabel={`${t(category.labelKey) || category.id} - ${category.count} services`}
              accessibilityRole="button"
            >
              <View style={[styles.categoryIcon, { backgroundColor: category.color + '20' }]}>
                <Icon name={category.icon} size={28} color={category.color} />
              </View>
              <Text style={styles.categoryLabel}>{t(category.labelKey) || category.id}</Text>
              <Badge style={[styles.categoryBadge, { backgroundColor: category.color }]}>
                {category.count}
              </Badge>
            </TouchableOpacity>
          ))}
        </View>
      </View>

      {/* Pending Requests */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('gov_pending') || 'Pending Requests'}</Text>
          <TouchableOpacity onPress={() => navigation.navigate('GovRequests')}>
            <Text style={styles.viewAll}>{t('view_all') || 'View All'}</Text>
          </TouchableOpacity>
        </View>
        {pendingRequests.map((request) => (
          <Surface key={request.id} style={styles.requestCard}>
            <View style={styles.requestInfo}>
              <Text style={styles.requestTitle}>{request.title}</Text>
              <Text style={styles.requestDate}>{request.date}</Text>
            </View>
            <View
              style={[styles.statusBadge, { backgroundColor: getStatusColor(request.status) + '20' }]}
            >
              <Text style={[styles.statusText, { color: getStatusColor(request.status) }]}>
                {request.status.toUpperCase()}
              </Text>
            </View>
          </Surface>
        ))}
      </View>

      {/* Quick Actions */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('gov_quick_actions') || 'Quick Actions'}</Text>
        <View style={styles.quickActions}>
          {[
            { icon: 'file-plus', label: t('gov_new_request') || 'New Request', screen: 'GovNewRequest' },
            { icon: 'message-text', label: t('gov_contact') || 'Contact Agency', screen: 'GovContact' },
            { icon: 'calendar-clock', label: t('gov_appointment') || 'Book Appointment', screen: 'GovAppointment' },
            { icon: 'bell-ring', label: t('gov_alerts') || 'Alerts', screen: 'GovAlerts' },
          ].map((action, index) => (
            <TouchableOpacity
              key={index}
              style={styles.quickActionBtn}
              onPress={() => navigation.navigate(action.screen)}
              accessibilityLabel={action.label}
              accessibilityRole="button"
            >
              <Icon name={action.icon} size={24} color="#00FF41" />
              <Text style={styles.quickActionLabel}>{action.label}</Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#888', fontSize: 14, marginTop: 4 },
  citizenCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 20, marginBottom: 20, borderWidth: 1, borderColor: '#FFD70040' },
  citizenCardHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 15 },
  citizenCardTitle: { color: '#FFD700', fontSize: 16, fontWeight: 'bold', marginLeft: 10 },
  citizenCardBody: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  citizenName: { color: '#fff', fontSize: 20, fontWeight: 'bold' },
  citizenIdNumber: { color: '#00FF41', fontSize: 14, marginTop: 4, fontFamily: 'monospace' },
  citizenStatus: { color: '#00FF41', fontSize: 12, marginTop: 8 },
  qrButton: { padding: 8 },
  searchBar: { backgroundColor: '#1A1A2E', marginBottom: 20, borderRadius: 12 },
  section: { marginBottom: 25 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  viewAll: { color: '#00FF41', fontSize: 14 },
  categoriesGrid: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  categoryCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, width: '48%', alignItems: 'center', marginBottom: 12 },
  categoryIcon: { width: 56, height: 56, borderRadius: 16, justifyContent: 'center', alignItems: 'center', marginBottom: 10 },
  categoryLabel: { color: '#fff', fontSize: 13, textAlign: 'center' },
  categoryBadge: { position: 'absolute', top: 8, right: 8 },
  requestCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 10 },
  requestInfo: { flex: 1 },
  requestTitle: { color: '#fff', fontSize: 15, fontWeight: '500' },
  requestDate: { color: '#888', fontSize: 12, marginTop: 4 },
  statusBadge: { paddingHorizontal: 12, paddingVertical: 6, borderRadius: 20 },
  statusText: { fontSize: 11, fontWeight: 'bold' },
  quickActions: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  quickActionBtn: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, width: '48%', alignItems: 'center', marginBottom: 12 },
  quickActionLabel: { color: '#fff', fontSize: 12, marginTop: 8, textAlign: 'center' },
});
