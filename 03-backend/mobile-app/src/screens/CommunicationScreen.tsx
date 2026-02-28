/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  COMMUNICATION SCREEN - Sovereign Messaging & Calls
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description End-to-end encrypted messaging platform with quantum-secured
 *   channels, video calls, group chats, government announcements,
 *   and community forums. Integrates with NEXUS Voces.
 *
 * @module screens/CommunicationScreen
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
  RefreshControl,
} from 'react-native';
import { Text, Surface, Searchbar, Avatar, Badge, FAB } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';

/**
 * CommunicationScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The sovereign messaging and communication screen
 */
export default function CommunicationScreen({ navigation }) {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [refreshing, setRefreshing] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeTab, setActiveTab] = useState('chats');

  const onRefresh = async () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 1500);
  };

  const tabs = [
    { key: 'chats', label: t('comm_chats') || 'Chats', icon: 'chat', badge: 5 },
    { key: 'calls', label: t('comm_calls') || 'Calls', icon: 'phone', badge: 0 },
    { key: 'channels', label: t('comm_channels') || 'Channels', icon: 'bullhorn', badge: 12 },
    { key: 'forums', label: t('comm_forums') || 'Forums', icon: 'forum', badge: 3 },
  ];

  const [conversations] = useState([
    {
      id: '1', name: 'Karakwine Taiaiake', lastMessage: 'The governance proposal looks great!',
      time: '2 min', unread: 2, online: true, avatar: 'KT', type: 'direct',
    },
    {
      id: '2', name: 'NEXUS Consejo', lastMessage: 'New proposal: Budget allocation Q2 2026',
      time: '15 min', unread: 8, online: false, avatar: 'NC', type: 'group',
    },
    {
      id: '3', name: 'Tekahionwake Dev Team', lastMessage: 'Deployed v3.9.0 to testnet',
      time: '1 hr', unread: 0, online: false, avatar: 'TD', type: 'group',
    },
    {
      id: '4', name: 'Aiakwiraien Sakokwe', lastMessage: 'Can you review the health module?',
      time: '3 hr', unread: 1, online: true, avatar: 'AS', type: 'direct',
    },
    {
      id: '5', name: 'Sovereign Citizens Forum', lastMessage: 'Discussion: Digital rights framework',
      time: '5 hr', unread: 0, online: false, avatar: 'SC', type: 'group',
    },
  ]);

  const [recentCalls] = useState([
    { id: '1', name: 'Karakwine Taiaiake', type: 'video', direction: 'outgoing', time: 'Today, 10:30 AM', duration: '15:42' },
    { id: '2', name: 'NEXUS Consejo', type: 'voice', direction: 'incoming', time: 'Today, 9:15 AM', duration: '32:10' },
    { id: '3', name: 'Aiakwiraien Sakokwe', type: 'video', direction: 'missed', time: 'Yesterday, 4:20 PM', duration: null },
  ]);

  const [channels] = useState([
    { id: '1', name: 'Government Announcements', subscribers: 72000, lastPost: '1 hr ago', icon: 'bullhorn', color: '#FFD700' },
    { id: '2', name: 'Tech Updates', subscribers: 15400, lastPost: '3 hr ago', icon: 'code-braces', color: '#00FFFF' },
    { id: '3', name: 'Community Events', subscribers: 45200, lastPost: '5 hr ago', icon: 'calendar-star', color: '#E040FB' },
    { id: '4', name: 'Security Alerts', subscribers: 68000, lastPost: '30 min ago', icon: 'shield-alert', color: '#FF4444' },
  ]);

  /**
   * Renders the appropriate avatar color based on conversation type
   * @param {string} type - direct or group
   * @returns {string} Hex color code
   */
  const getAvatarColor = (type) => {
    return type === 'direct' ? '#00FF41' : '#7C4DFF';
  };

  const renderChats = () => (
    <>
      {conversations.map((conv) => (
        <TouchableOpacity
          key={conv.id}
          style={styles.chatRow}
          onPress={() => navigation.navigate('ChatDetail', { chatId: conv.id })}
          accessibilityLabel={`Chat with ${conv.name}, ${conv.unread > 0 ? `${conv.unread} unread messages` : 'no unread messages'}`}
          accessibilityRole="button"
        >
          <View style={styles.avatarContainer}>
            <Avatar.Text
              size={48}
              label={conv.avatar}
              style={[styles.chatAvatar, { backgroundColor: getAvatarColor(conv.type) }]}
              labelStyle={{ fontSize: 16 }}
            />
            {conv.online && <View style={styles.onlineIndicator} />}
          </View>
          <View style={styles.chatInfo}>
            <View style={styles.chatHeader}>
              <Text style={styles.chatName} numberOfLines={1}>{conv.name}</Text>
              <Text style={styles.chatTime}>{conv.time}</Text>
            </View>
            <View style={styles.chatFooter}>
              <Text style={styles.chatMessage} numberOfLines={1}>{conv.lastMessage}</Text>
              {conv.unread > 0 && (
                <Badge style={styles.unreadBadge}>{conv.unread}</Badge>
              )}
            </View>
          </View>
        </TouchableOpacity>
      ))}
    </>
  );

  const renderCalls = () => (
    <>
      {recentCalls.map((call) => (
        <TouchableOpacity
          key={call.id}
          style={styles.callRow}
          accessibilityLabel={`${call.direction} ${call.type} call with ${call.name}`}
          accessibilityRole="button"
        >
          <Icon
            name={call.type === 'video' ? 'video' : 'phone'}
            size={24}
            color={call.direction === 'missed' ? '#FF4444' : '#00FF41'}
          />
          <View style={styles.callInfo}>
            <Text style={styles.callName}>{call.name}</Text>
            <View style={styles.callMeta}>
              <Icon
                name={call.direction === 'incoming' ? 'arrow-down-left' : call.direction === 'outgoing' ? 'arrow-up-right' : 'phone-missed'}
                size={14}
                color={call.direction === 'missed' ? '#FF4444' : '#888'}
              />
              <Text style={styles.callTime}>{call.time}</Text>
              {call.duration && <Text style={styles.callDuration}>{call.duration}</Text>}
            </View>
          </View>
          <TouchableOpacity style={styles.callAction} accessibilityLabel={`Call ${call.name}`}>
            <Icon name="phone" size={20} color="#00FF41" />
          </TouchableOpacity>
        </TouchableOpacity>
      ))}
    </>
  );

  const renderChannels = () => (
    <>
      {channels.map((channel) => (
        <TouchableOpacity
          key={channel.id}
          style={styles.channelRow}
          onPress={() => navigation.navigate('ChannelDetail', { channelId: channel.id })}
          accessibilityLabel={`${channel.name} channel, ${channel.subscribers.toLocaleString()} subscribers`}
          accessibilityRole="button"
        >
          <View style={[styles.channelIcon, { backgroundColor: channel.color + '20' }]}>
            <Icon name={channel.icon} size={24} color={channel.color} />
          </View>
          <View style={styles.channelInfo}>
            <Text style={styles.channelName}>{channel.name}</Text>
            <Text style={styles.channelMeta}>
              {channel.subscribers.toLocaleString()} {t('comm_subscribers') || 'subscribers'} · {channel.lastPost}
            </Text>
          </View>
          <Icon name="chevron-right" size={20} color="#555" />
        </TouchableOpacity>
      ))}
    </>
  );

  return (
    <View style={styles.container}>
      <ScrollView
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
        accessibilityLabel={t('comm_screen_title') || 'Communication Screen'}
      >
        {/* Header */}
        <View style={styles.header}>
          <Text style={styles.headerTitle}>{t('comm_title') || 'Messages'}</Text>
          <Text style={styles.headerSubtitle}>{t('comm_subtitle') || 'Quantum-Encrypted Communication'}</Text>
        </View>

        {/* Search */}
        <Searchbar
          placeholder={t('comm_search') || 'Search conversations...'}
          onChangeText={setSearchQuery}
          value={searchQuery}
          style={styles.searchBar}
          inputStyle={{ color: '#fff' }}
          iconColor="#888"
          placeholderTextColor="#666"
          accessibilityLabel="Search conversations"
        />

        {/* Tabs */}
        <View style={styles.tabsRow}>
          {tabs.map((tab) => (
            <TouchableOpacity
              key={tab.key}
              style={[styles.tab, activeTab === tab.key && styles.tabActive]}
              onPress={() => setActiveTab(tab.key)}
              accessibilityLabel={`${tab.label}${tab.badge > 0 ? `, ${tab.badge} new` : ''}`}
              accessibilityRole="tab"
              accessibilityState={{ selected: activeTab === tab.key }}
            >
              <Icon name={tab.icon} size={20} color={activeTab === tab.key ? '#00FF41' : '#888'} />
              <Text style={[styles.tabLabel, activeTab === tab.key && styles.tabLabelActive]}>
                {tab.label}
              </Text>
              {tab.badge > 0 && <Badge style={styles.tabBadge} size={18}>{tab.badge}</Badge>}
            </TouchableOpacity>
          ))}
        </View>

        {/* Encryption Banner */}
        <Surface style={styles.encryptionBanner}>
          <Icon name="lock" size={16} color="#00FF41" />
          <Text style={styles.encryptionText}>
            {t('comm_encrypted') || 'All messages are end-to-end encrypted with quantum-safe algorithms'}
          </Text>
        </Surface>

        {/* Content */}
        <View style={styles.content}>
          {activeTab === 'chats' && renderChats()}
          {activeTab === 'calls' && renderCalls()}
          {activeTab === 'channels' && renderChannels()}
          {activeTab === 'forums' && (
            <Surface style={styles.comingSoon}>
              <Icon name="forum" size={48} color="#333" />
              <Text style={styles.comingSoonText}>{t('comm_forums_soon') || 'Community Forums'}</Text>
              <Text style={styles.comingSoonDesc}>{t('comm_forums_desc') || 'Launching March 2026'}</Text>
            </Surface>
          )}
        </View>

        <View style={{ height: 80 }} />
      </ScrollView>

      {/* New Message FAB */}
      <FAB
        icon="message-plus"
        style={styles.fab}
        onPress={() => navigation.navigate('NewChat')}
        color="#000"
        accessibilityLabel="Start new conversation"
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F' },
  header: { padding: 16, paddingBottom: 0 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#888', fontSize: 14, marginTop: 4 },
  searchBar: { backgroundColor: '#1A1A2E', margin: 16, borderRadius: 12 },
  tabsRow: { flexDirection: 'row', paddingHorizontal: 16, marginBottom: 8 },
  tab: { flex: 1, flexDirection: 'row', alignItems: 'center', justifyContent: 'center', paddingVertical: 12, borderBottomWidth: 2, borderBottomColor: 'transparent' },
  tabActive: { borderBottomColor: '#00FF41' },
  tabLabel: { color: '#888', fontSize: 13, marginLeft: 6 },
  tabLabelActive: { color: '#00FF41', fontWeight: '600' },
  tabBadge: { backgroundColor: '#FF4444', marginLeft: 4 },
  encryptionBanner: { backgroundColor: '#00FF4110', marginHorizontal: 16, borderRadius: 8, padding: 10, flexDirection: 'row', alignItems: 'center', marginBottom: 12 },
  encryptionText: { color: '#00FF41', fontSize: 11, marginLeft: 8, flex: 1 },
  content: { paddingHorizontal: 16 },
  chatRow: { flexDirection: 'row', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#1A1A2E' },
  avatarContainer: { position: 'relative' },
  chatAvatar: {},
  onlineIndicator: { position: 'absolute', bottom: 2, right: 2, width: 12, height: 12, borderRadius: 6, backgroundColor: '#00FF41', borderWidth: 2, borderColor: '#0A0A0F' },
  chatInfo: { flex: 1, marginLeft: 12 },
  chatHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  chatName: { color: '#fff', fontSize: 16, fontWeight: '600', flex: 1 },
  chatTime: { color: '#888', fontSize: 12 },
  chatFooter: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginTop: 4 },
  chatMessage: { color: '#888', fontSize: 14, flex: 1 },
  unreadBadge: { backgroundColor: '#00FF41' },
  callRow: { flexDirection: 'row', alignItems: 'center', paddingVertical: 14, borderBottomWidth: 1, borderBottomColor: '#1A1A2E' },
  callInfo: { flex: 1, marginLeft: 12 },
  callName: { color: '#fff', fontSize: 16, fontWeight: '500' },
  callMeta: { flexDirection: 'row', alignItems: 'center', marginTop: 4 },
  callTime: { color: '#888', fontSize: 12, marginLeft: 4 },
  callDuration: { color: '#888', fontSize: 12, marginLeft: 8 },
  callAction: { padding: 10 },
  channelRow: { flexDirection: 'row', alignItems: 'center', paddingVertical: 14, borderBottomWidth: 1, borderBottomColor: '#1A1A2E' },
  channelIcon: { width: 48, height: 48, borderRadius: 14, justifyContent: 'center', alignItems: 'center' },
  channelInfo: { flex: 1, marginLeft: 12 },
  channelName: { color: '#fff', fontSize: 16, fontWeight: '600' },
  channelMeta: { color: '#888', fontSize: 12, marginTop: 4 },
  comingSoon: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 40, alignItems: 'center', marginTop: 20 },
  comingSoonText: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginTop: 16 },
  comingSoonDesc: { color: '#888', fontSize: 14, marginTop: 8 },
  fab: { position: 'absolute', bottom: 20, right: 20, backgroundColor: '#00FF41', borderRadius: 28 },
});
