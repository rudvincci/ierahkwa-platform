/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  PROFILE SCREEN - User Identity & Account Management
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Displays user identity information including FutureWampum ID,
 *   sovereign citizenship details, wallet addresses, KYC status,
 *   and account management options.
 *
 * @module screens/ProfileScreen
 * @requires react-native
 * @requires react-native-paper
 * @requires react-i18next
 */

import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Image,
} from 'react-native';
import { Text, Surface, Button, Avatar, Divider } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';

/**
 * ProfileScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The user profile and identity screen
 */
export default function ProfileScreen({ navigation }) {
  const { t } = useTranslation();
  const { user } = useAuthStore();

  const [kycStatus] = useState({
    level: 'LEVEL_3',
    verified: true,
    lastVerified: '2026-01-15',
    documents: [
      { type: 'Identity', status: 'verified' },
      { type: 'Address', status: 'verified' },
      { type: 'Biometric', status: 'verified' },
    ],
  });

  const [wallets] = useState([
    { chain: 'Mamey Mainnet', address: '0x7a3B...4f2E', balance: '12,450.00 ISB', isPrimary: true },
    { chain: 'Ethereum', address: '0x1c9D...8b3A', balance: '0.45 ETH', isPrimary: false },
    { chain: 'Polygon', address: '0x4eF1...2d9C', balance: '1,200 MATIC', isPrimary: false },
  ]);

  const [citizenBadges] = useState([
    { name: 'Sovereign Citizen', icon: 'shield-star', color: '#FFD700', earned: '2025-06-15' },
    { name: 'Early Adopter', icon: 'rocket-launch', color: '#00FF41', earned: '2025-06-15' },
    { name: 'Governance Voter', icon: 'vote', color: '#7C4DFF', earned: '2025-08-20' },
    { name: 'Community Builder', icon: 'account-group', color: '#00FFFF', earned: '2025-10-10' },
  ]);

  const profileMenuItems = [
    { icon: 'pencil', label: t('profile_edit') || 'Edit Profile', screen: 'EditProfile', color: '#00FF41' },
    { icon: 'card-account-details', label: t('profile_documents') || 'Documents', screen: 'Documents', color: '#00FFFF' },
    { icon: 'history', label: t('profile_activity') || 'Activity Log', screen: 'ActivityLog', color: '#FFD700' },
    { icon: 'link-variant', label: t('profile_connections') || 'Connected Apps', screen: 'ConnectedApps', color: '#E040FB' },
    { icon: 'download', label: t('profile_export') || 'Export Data', screen: 'ExportData', color: '#FF9100' },
    { icon: 'delete', label: t('profile_delete') || 'Delete Account', screen: 'DeleteAccount', color: '#FF4444' },
  ];

  return (
    <ScrollView
      style={styles.container}
      accessibilityLabel={t('profile_screen_title') || 'Profile Screen'}
    >
      {/* Profile Header */}
      <Surface style={styles.profileCard}>
        <View style={styles.avatarSection}>
          <Avatar.Text
            size={80}
            label={`${user?.firstName?.[0] || 'U'}${user?.lastName?.[0] || ''}`}
            style={styles.avatar}
            labelStyle={styles.avatarLabel}
          />
          <TouchableOpacity
            style={styles.editAvatarBtn}
            onPress={() => navigation.navigate('EditAvatar')}
            accessibilityLabel="Change profile photo"
            accessibilityRole="button"
          >
            <Icon name="camera" size={16} color="#000" />
          </TouchableOpacity>
        </View>
        <Text style={styles.userName}>
          {user?.firstName || 'Sovereign'} {user?.lastName || 'Citizen'}
        </Text>
        <Text style={styles.userHandle}>@{user?.username || 'citizen'}</Text>

        {/* FutureWampum ID */}
        <Surface style={styles.fwidCard} accessibilityLabel="FutureWampum digital identity">
          <View style={styles.fwidHeader}>
            <Icon name="fingerprint" size={20} color="#FFD700" />
            <Text style={styles.fwidTitle}>FutureWampum ID</Text>
          </View>
          <Text style={styles.fwidValue}>{user?.citizenId || 'FWID-IK-2026-000001'}</Text>
          <View style={styles.fwidVerified}>
            <Icon name="check-decagram" size={16} color="#00FF41" />
            <Text style={styles.fwidVerifiedText}>{t('profile_verified') || 'Verified on Mamey Blockchain'}</Text>
          </View>
        </Surface>
      </Surface>

      {/* KYC Status */}
      <Surface style={styles.sectionCard}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('profile_kyc') || 'KYC Verification'}</Text>
          <View style={[styles.kycBadge, { backgroundColor: '#00FF4120' }]}>
            <Text style={styles.kycBadgeText}>{kycStatus.level}</Text>
          </View>
        </View>
        {kycStatus.documents.map((doc, index) => (
          <View key={index} style={styles.kycRow}>
            <View style={styles.kycDocInfo}>
              <Icon
                name={doc.status === 'verified' ? 'check-circle' : 'clock-outline'}
                size={18}
                color={doc.status === 'verified' ? '#00FF41' : '#FFD700'}
              />
              <Text style={styles.kycDocName}>{doc.type}</Text>
            </View>
            <Text style={[styles.kycDocStatus, { color: doc.status === 'verified' ? '#00FF41' : '#FFD700' }]}>
              {doc.status.toUpperCase()}
            </Text>
          </View>
        ))}
      </Surface>

      {/* Connected Wallets */}
      <Surface style={styles.sectionCard}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('profile_wallets') || 'Connected Wallets'}</Text>
          <TouchableOpacity
            onPress={() => navigation.navigate('AddWallet')}
            accessibilityLabel="Add a new wallet"
          >
            <Icon name="plus-circle" size={22} color="#00FF41" />
          </TouchableOpacity>
        </View>
        {wallets.map((wallet, index) => (
          <TouchableOpacity
            key={index}
            style={styles.walletRow}
            onPress={() => navigation.navigate('WalletDetail', { address: wallet.address })}
            accessibilityLabel={`${wallet.chain} wallet, balance: ${wallet.balance}`}
            accessibilityRole="button"
          >
            <View style={styles.walletInfo}>
              <View style={styles.walletChainRow}>
                <Text style={styles.walletChain}>{wallet.chain}</Text>
                {wallet.isPrimary && (
                  <View style={styles.primaryBadge}>
                    <Text style={styles.primaryBadgeText}>PRIMARY</Text>
                  </View>
                )}
              </View>
              <Text style={styles.walletAddress}>{wallet.address}</Text>
            </View>
            <Text style={styles.walletBalance}>{wallet.balance}</Text>
          </TouchableOpacity>
        ))}
      </Surface>

      {/* Citizen Badges */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('profile_badges') || 'Citizen Badges'}</Text>
        <View style={styles.badgesGrid}>
          {citizenBadges.map((badge, index) => (
            <View
              key={index}
              style={styles.badgeCard}
              accessibilityLabel={`${badge.name} badge, earned ${badge.earned}`}
            >
              <View style={[styles.badgeIcon, { backgroundColor: badge.color + '20' }]}>
                <Icon name={badge.icon} size={24} color={badge.color} />
              </View>
              <Text style={styles.badgeName}>{badge.name}</Text>
              <Text style={styles.badgeDate}>{badge.earned}</Text>
            </View>
          ))}
        </View>
      </Surface>

      {/* Menu Items */}
      <Surface style={styles.sectionCard}>
        {profileMenuItems.map((item, index) => (
          <TouchableOpacity
            key={index}
            style={styles.menuRow}
            onPress={() => navigation.navigate(item.screen)}
            accessibilityLabel={item.label}
            accessibilityRole="button"
          >
            <View style={styles.menuLeft}>
              <Icon name={item.icon} size={20} color={item.color} />
              <Text style={[styles.menuLabel, item.color === '#FF4444' && { color: '#FF4444' }]}>
                {item.label}
              </Text>
            </View>
            <Icon name="chevron-right" size={20} color="#555" />
          </TouchableOpacity>
        ))}
      </Surface>

      <View style={{ height: 40 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  profileCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 24, alignItems: 'center', marginBottom: 16 },
  avatarSection: { position: 'relative', marginBottom: 16 },
  avatar: { backgroundColor: '#00FF41' },
  avatarLabel: { fontSize: 32, fontWeight: 'bold' },
  editAvatarBtn: { position: 'absolute', bottom: 0, right: 0, backgroundColor: '#00FF41', borderRadius: 14, width: 28, height: 28, justifyContent: 'center', alignItems: 'center' },
  userName: { color: '#fff', fontSize: 24, fontWeight: 'bold' },
  userHandle: { color: '#888', fontSize: 14, marginTop: 4, marginBottom: 16 },
  fwidCard: { backgroundColor: '#0A0A0F', borderRadius: 12, padding: 16, width: '100%', borderWidth: 1, borderColor: '#FFD70040' },
  fwidHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 8 },
  fwidTitle: { color: '#FFD700', fontSize: 14, fontWeight: 'bold', marginLeft: 8 },
  fwidValue: { color: '#fff', fontSize: 18, fontFamily: 'monospace', fontWeight: 'bold' },
  fwidVerified: { flexDirection: 'row', alignItems: 'center', marginTop: 8 },
  fwidVerifiedText: { color: '#00FF41', fontSize: 12, marginLeft: 6 },
  sectionCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 16, marginBottom: 16 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 },
  sectionTitle: { color: '#00FF41', fontSize: 14, fontWeight: 'bold', textTransform: 'uppercase', letterSpacing: 1, marginBottom: 12 },
  kycBadge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 8 },
  kycBadgeText: { color: '#00FF41', fontSize: 12, fontWeight: 'bold' },
  kycRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 10, borderBottomWidth: 1, borderBottomColor: '#222' },
  kycDocInfo: { flexDirection: 'row', alignItems: 'center' },
  kycDocName: { color: '#fff', fontSize: 14, marginLeft: 10 },
  kycDocStatus: { fontSize: 12, fontWeight: 'bold' },
  walletRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#222' },
  walletInfo: { flex: 1 },
  walletChainRow: { flexDirection: 'row', alignItems: 'center' },
  walletChain: { color: '#fff', fontSize: 14, fontWeight: '600' },
  primaryBadge: { backgroundColor: '#00FF4120', paddingHorizontal: 8, paddingVertical: 2, borderRadius: 6, marginLeft: 8 },
  primaryBadgeText: { color: '#00FF41', fontSize: 10, fontWeight: 'bold' },
  walletAddress: { color: '#888', fontSize: 12, marginTop: 2, fontFamily: 'monospace' },
  walletBalance: { color: '#FFD700', fontSize: 14, fontWeight: '600' },
  badgesGrid: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  badgeCard: { width: '48%', alignItems: 'center', marginBottom: 16 },
  badgeIcon: { width: 52, height: 52, borderRadius: 16, justifyContent: 'center', alignItems: 'center', marginBottom: 8 },
  badgeName: { color: '#fff', fontSize: 12, textAlign: 'center', fontWeight: '500' },
  badgeDate: { color: '#888', fontSize: 10, marginTop: 2 },
  menuRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 14, borderBottomWidth: 1, borderBottomColor: '#222' },
  menuLeft: { flexDirection: 'row', alignItems: 'center' },
  menuLabel: { color: '#fff', fontSize: 15, marginLeft: 12 },
});
