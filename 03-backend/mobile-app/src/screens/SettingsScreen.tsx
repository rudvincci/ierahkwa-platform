/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  SETTINGS SCREEN - Language, Theme, Notifications
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description User preferences and configuration screen including language
 *   selection (English, Spanish, Mohawk, Taino), dark/light theme toggle,
 *   notification preferences, security settings, and app information.
 *
 * @module screens/SettingsScreen
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
  Switch,
  Alert,
} from 'react-native';
import { Text, Surface, Divider, RadioButton } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';

/** @typedef {Object} Language */
const LANGUAGES = [
  { code: 'en', name: 'English', native: 'English', flag: 'US' },
  { code: 'es', name: 'Spanish', native: 'Espanol', flag: 'PR' },
  { code: 'moh', name: 'Mohawk', native: "Kanien'keha", flag: 'IK' },
  { code: 'tai', name: 'Taino', native: 'Taino', flag: 'TA' },
];

/**
 * SettingsScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The settings and preferences screen
 */
export default function SettingsScreen({ navigation }) {
  const { t, i18n } = useTranslation();
  const { user } = useAuthStore();
  const [darkMode, setDarkMode] = useState(true);
  const [notifications, setNotifications] = useState({
    transactions: true,
    governance: true,
    security: true,
    health: false,
    education: true,
    marketing: false,
  });
  const [biometricEnabled, setBiometricEnabled] = useState(true);
  const [selectedLanguage, setSelectedLanguage] = useState(i18n.language || 'en');
  const [showLanguagePicker, setShowLanguagePicker] = useState(false);

  /**
   * Changes the application language
   * @param {string} langCode - ISO language code
   */
  const changeLanguage = (langCode) => {
    i18n.changeLanguage(langCode);
    setSelectedLanguage(langCode);
    setShowLanguagePicker(false);
  };

  /**
   * Toggles a specific notification setting
   * @param {string} key - Notification preference key
   */
  const toggleNotification = (key) => {
    setNotifications((prev) => ({ ...prev, [key]: !prev[key] }));
  };

  /**
   * Renders a single settings row item
   * @param {Object} params - Row parameters
   * @returns {JSX.Element}
   */
  const renderSettingsRow = ({ icon, label, value, onPress, rightElement, color = '#fff' }) => (
    <TouchableOpacity
      style={styles.settingsRow}
      onPress={onPress}
      accessibilityLabel={label}
      accessibilityRole="button"
    >
      <View style={styles.settingsLeft}>
        <Icon name={icon} size={22} color={color} />
        <Text style={styles.settingsLabel}>{label}</Text>
      </View>
      {rightElement || (
        <View style={styles.settingsRight}>
          {value && <Text style={styles.settingsValue}>{value}</Text>}
          <Icon name="chevron-right" size={20} color="#555" />
        </View>
      )}
    </TouchableOpacity>
  );

  return (
    <ScrollView
      style={styles.container}
      accessibilityLabel={t('settings_screen_title') || 'Settings Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>{t('settings') || 'Settings'}</Text>
      </View>

      {/* Language Section */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('language') || 'Language'}</Text>
        {LANGUAGES.map((lang) => (
          <TouchableOpacity
            key={lang.code}
            style={styles.languageRow}
            onPress={() => changeLanguage(lang.code)}
            accessibilityLabel={`Select ${lang.name} language`}
            accessibilityRole="radio"
            accessibilityState={{ checked: selectedLanguage === lang.code }}
          >
            <View style={styles.languageInfo}>
              <Text style={styles.languageName}>{lang.name}</Text>
              <Text style={styles.languageNative}>{lang.native}</Text>
            </View>
            <RadioButton
              value={lang.code}
              status={selectedLanguage === lang.code ? 'checked' : 'unchecked'}
              onPress={() => changeLanguage(lang.code)}
              color="#00FF41"
            />
          </TouchableOpacity>
        ))}
      </Surface>

      {/* Appearance */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('theme') || 'Appearance'}</Text>
        {renderSettingsRow({
          icon: 'theme-light-dark',
          label: t('settings_dark_mode') || 'Dark Mode',
          rightElement: (
            <Switch
              value={darkMode}
              onValueChange={setDarkMode}
              trackColor={{ false: '#333', true: '#00FF4180' }}
              thumbColor={darkMode ? '#00FF41' : '#888'}
              accessibilityLabel="Toggle dark mode"
            />
          ),
        })}
        {renderSettingsRow({
          icon: 'format-size',
          label: t('settings_font_size') || 'Font Size',
          value: 'Medium',
          onPress: () => navigation.navigate('FontSettings'),
        })}
      </Surface>

      {/* Notifications */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('notifications') || 'Notifications'}</Text>
        {Object.entries(notifications).map(([key, enabled]) => (
          <View key={key} style={styles.notifRow}>
            <Text style={styles.notifLabel}>
              {t(`notif_${key}`) || key.charAt(0).toUpperCase() + key.slice(1)}
            </Text>
            <Switch
              value={enabled}
              onValueChange={() => toggleNotification(key)}
              trackColor={{ false: '#333', true: '#00FF4180' }}
              thumbColor={enabled ? '#00FF41' : '#888'}
              accessibilityLabel={`Toggle ${key} notifications`}
            />
          </View>
        ))}
      </Surface>

      {/* Security */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('security') || 'Security'}</Text>
        {renderSettingsRow({
          icon: 'fingerprint',
          label: t('settings_biometric') || 'Biometric Authentication',
          rightElement: (
            <Switch
              value={biometricEnabled}
              onValueChange={setBiometricEnabled}
              trackColor={{ false: '#333', true: '#00FF4180' }}
              thumbColor={biometricEnabled ? '#00FF41' : '#888'}
              accessibilityLabel="Toggle biometric authentication"
            />
          ),
        })}
        {renderSettingsRow({
          icon: 'lock-reset',
          label: t('settings_change_pin') || 'Change PIN',
          onPress: () => navigation.navigate('ChangePin'),
        })}
        {renderSettingsRow({
          icon: 'key-variant',
          label: t('settings_backup_keys') || 'Backup Recovery Keys',
          onPress: () => navigation.navigate('BackupKeys'),
        })}
        {renderSettingsRow({
          icon: 'two-factor-authentication',
          label: t('settings_2fa') || 'Two-Factor Authentication',
          value: 'Enabled',
          onPress: () => navigation.navigate('TwoFactor'),
        })}
      </Surface>

      {/* Network */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('network_status') || 'Network'}</Text>
        {renderSettingsRow({
          icon: 'server-network',
          label: t('settings_rpc') || 'RPC Endpoint',
          value: 'Mamey Mainnet',
          onPress: () => navigation.navigate('NetworkSettings'),
        })}
        {renderSettingsRow({
          icon: 'cube-scan',
          label: t('settings_explorer') || 'Block Explorer',
          onPress: () => navigation.navigate('Explorer'),
        })}
      </Surface>

      {/* About */}
      <Surface style={styles.sectionCard}>
        <Text style={styles.sectionTitle}>{t('about') || 'About'}</Text>
        {renderSettingsRow({
          icon: 'information',
          label: t('settings_version') || 'Version',
          value: 'v3.9.0',
        })}
        {renderSettingsRow({
          icon: 'file-document',
          label: t('settings_terms') || 'Terms of Service',
          onPress: () => navigation.navigate('Terms'),
        })}
        {renderSettingsRow({
          icon: 'shield-account',
          label: t('settings_privacy') || 'Privacy Policy',
          onPress: () => navigation.navigate('Privacy'),
        })}
        {renderSettingsRow({
          icon: 'help-circle',
          label: t('settings_support') || 'Support',
          onPress: () => navigation.navigate('Support'),
        })}
      </Surface>

      {/* Logout */}
      <TouchableOpacity
        style={styles.logoutButton}
        onPress={() =>
          Alert.alert(
            t('settings_logout_title') || 'Log Out',
            t('settings_logout_message') || 'Are you sure you want to log out?',
            [
              { text: t('cancel') || 'Cancel', style: 'cancel' },
              { text: t('settings_logout') || 'Log Out', style: 'destructive', onPress: () => {} },
            ]
          )
        }
        accessibilityLabel="Log out of your account"
        accessibilityRole="button"
      >
        <Icon name="logout" size={20} color="#FF4444" />
        <Text style={styles.logoutText}>{t('settings_logout') || 'Log Out'}</Text>
      </TouchableOpacity>

      <View style={{ height: 40 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  sectionCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 16, marginBottom: 16 },
  sectionTitle: { color: '#00FF41', fontSize: 14, fontWeight: 'bold', marginBottom: 12, textTransform: 'uppercase', letterSpacing: 1 },
  settingsRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 14, borderBottomWidth: 1, borderBottomColor: '#222' },
  settingsLeft: { flexDirection: 'row', alignItems: 'center' },
  settingsLabel: { color: '#fff', fontSize: 15, marginLeft: 12 },
  settingsRight: { flexDirection: 'row', alignItems: 'center' },
  settingsValue: { color: '#888', fontSize: 14, marginRight: 8 },
  languageRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#222' },
  languageInfo: {},
  languageName: { color: '#fff', fontSize: 15 },
  languageNative: { color: '#888', fontSize: 12, marginTop: 2 },
  notifRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#222' },
  notifLabel: { color: '#fff', fontSize: 15 },
  logoutButton: { flexDirection: 'row', alignItems: 'center', justifyContent: 'center', paddingVertical: 16, marginTop: 8 },
  logoutText: { color: '#FF4444', fontSize: 16, fontWeight: '600', marginLeft: 8 },
});
