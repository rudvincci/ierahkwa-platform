/**
 * Rewards Screen - Gamification & Achievements
 */

import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { useTranslation } from 'react-i18next';
import api from '../services/api';

const USER_ADDRESS = '0x' + Math.random().toString(16).slice(2, 42);

export default function RewardsScreen() {
  const { t } = useTranslation();
  const [profile, setProfile] = useState(null);
  const [achievements, setAchievements] = useState([]);
  const [leaderboard, setLeaderboard] = useState([]);
  const [canClaim, setCanClaim] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [profileData, achievementsData, leaderboardData] = await Promise.all([
        api.getProfile(USER_ADDRESS),
        api.getAchievements(),
        api.getLeaderboard(),
      ]);
      
      setProfile(profileData.profile);
      setAchievements(achievementsData.achievements || []);
      setLeaderboard(leaderboardData.leaderboard || []);
      
      // Check if can claim
      const lastDaily = profileData.profile.lastDaily || 0;
      setCanClaim(Date.now() - lastDaily > 86400000);
    } catch (e) {
      console.error('Error loading data:', e);
    }
  };

  const claimDaily = async () => {
    try {
      const result = await api.claimDailyReward(USER_ADDRESS);
      Alert.alert(
        '🎉 Reward Claimed!',
        `+${result.reward} points!\nStreak: Day ${result.streak}`
      );
      setCanClaim(false);
      loadData();
    } catch (e) {
      Alert.alert('Error', e.message);
    }
  };

  const dailyRewards = { 1: 10, 2: 20, 3: 30, 4: 40, 5: 50, 6: 75, 7: 100 };
  const nextReward = dailyRewards[Math.min((profile?.dailyStreak || 0) + 1, 7)] || 100;

  return (
    <ScrollView style={styles.container}>
      {/* Profile Card */}
      <View style={styles.profileCard}>
        <View style={styles.avatar}>
          <Text style={styles.avatarText}>👤</Text>
        </View>
        <View style={styles.profileInfo}>
          <Text style={styles.address}>
            {USER_ADDRESS.slice(0, 8)}...{USER_ADDRESS.slice(-6)}
          </Text>
          <View style={styles.levelBadge}>
            <Text style={styles.levelText}>⭐ {t('level')} {profile?.level || 1}</Text>
          </View>
          <Text style={styles.pointsText}>
            {(profile?.points || 0).toLocaleString()} {t('points')}
          </Text>
          <View style={styles.xpBar}>
            <View style={[styles.xpFill, { width: `${(profile?.points || 0) % 1000 / 10}%` }]} />
          </View>
        </View>
      </View>

      {/* Stats Row */}
      <View style={styles.statsRow}>
        <StatBox icon="🏆" value={profile?.achievements?.length || 0} label={t('achievements')} />
        <StatBox icon="🔥" value={profile?.dailyStreak || 0} label="Streak" />
        <StatBox icon="📊" value={profile?.level || 1} label={t('level')} />
      </View>

      {/* Daily Reward */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>🎁 {t('daily_rewards')}</Text>
        <View style={styles.dailyCard}>
          <Text style={styles.dailyIcon}>🎁</Text>
          <Text style={styles.dailyPoints}>+{nextReward} PTS</Text>
          <Text style={styles.dailyStreak}>
            Day {Math.min((profile?.dailyStreak || 0) + 1, 7)} Bonus!
          </Text>
          
          <View style={styles.streakDays}>
            {[1, 2, 3, 4, 5, 6, 7].map(day => (
              <View
                key={day}
                style={[
                  styles.streakDay,
                  day <= (profile?.dailyStreak || 0) && styles.streakDayCompleted,
                  day === (profile?.dailyStreak || 0) + 1 && styles.streakDayActive,
                ]}
              >
                <Text style={styles.streakDayText}>{day}</Text>
              </View>
            ))}
          </View>

          <TouchableOpacity
            style={[styles.claimBtn, !canClaim && styles.claimBtnDisabled]}
            onPress={claimDaily}
            disabled={!canClaim}
          >
            <Text style={styles.claimBtnText}>
              {canClaim ? `🎁 ${t('claim')}` : '✓ Claimed Today'}
            </Text>
          </TouchableOpacity>
        </View>
      </View>

      {/* Achievements */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>🏆 {t('achievements')}</Text>
        <View style={styles.achievementsGrid}>
          {achievements.map(ach => {
            const earned = profile?.achievements?.includes(ach.id);
            return (
              <View key={ach.id} style={[styles.achievementCard, earned && styles.achievementEarned]}>
                <Text style={styles.achievementIcon}>{ach.icon}</Text>
                <Text style={styles.achievementName}>{earned ? '✓ ' : ''}{ach.name}</Text>
                <Text style={styles.achievementDesc}>{ach.description}</Text>
                <Text style={styles.achievementPoints}>+{ach.points}</Text>
              </View>
            );
          })}
        </View>
      </View>

      {/* Leaderboard */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>🏅 {t('leaderboard')}</Text>
        {leaderboard.slice(0, 10).map((user, i) => (
          <View key={i} style={styles.leaderboardItem}>
            <View style={[styles.rankBadge, i < 3 && styles[`rank${i + 1}`]]}>
              <Text style={styles.rankText}>
                {i === 0 ? '👑' : i === 1 ? '🥈' : i === 2 ? '🥉' : i + 1}
              </Text>
            </View>
            <View style={styles.leaderInfo}>
              <Text style={styles.leaderLevel}>{t('level')} {user.level}</Text>
              <Text style={styles.leaderAddress}>
                {user.address.slice(0, 8)}...{user.address.slice(-6)}
              </Text>
            </View>
            <Text style={styles.leaderPoints}>{user.points.toLocaleString()}</Text>
          </View>
        ))}
      </View>
    </ScrollView>
  );
}

function StatBox({ icon, value, label }) {
  return (
    <View style={styles.statBox}>
      <Text style={styles.statIcon}>{icon}</Text>
      <Text style={styles.statValue}>{value}</Text>
      <Text style={styles.statLabel}>{label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0a0e17' },
  profileCard: {
    flexDirection: 'row',
    margin: 15,
    padding: 20,
    backgroundColor: '#1a1f2e',
    borderRadius: 20,
    borderWidth: 2,
    borderColor: '#FF6B35',
  },
  avatar: {
    width: 80,
    height: 80,
    borderRadius: 40,
    backgroundColor: '#FF6B35',
    justifyContent: 'center',
    alignItems: 'center',
  },
  avatarText: { fontSize: 40 },
  profileInfo: { flex: 1, marginLeft: 15 },
  address: { color: '#fff', fontWeight: 'bold', fontSize: 16 },
  levelBadge: {
    alignSelf: 'flex-start',
    backgroundColor: 'rgba(255,215,0,0.2)',
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 12,
    marginVertical: 8,
  },
  levelText: { color: '#FFD700', fontWeight: 'bold' },
  pointsText: { color: '#888' },
  xpBar: { height: 8, backgroundColor: 'rgba(255,255,255,0.1)', borderRadius: 4, marginTop: 8 },
  xpFill: { height: '100%', backgroundColor: '#FF6B35', borderRadius: 4 },
  statsRow: { flexDirection: 'row', paddingHorizontal: 15, gap: 10 },
  statBox: { flex: 1, backgroundColor: '#1a1f2e', borderRadius: 12, padding: 15, alignItems: 'center' },
  statIcon: { fontSize: 24, marginBottom: 5 },
  statValue: { color: '#FFD700', fontSize: 20, fontWeight: 'bold' },
  statLabel: { color: '#888', fontSize: 12 },
  section: { padding: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  dailyCard: {
    backgroundColor: '#1a1f2e',
    borderRadius: 20,
    padding: 25,
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#00FF41',
    borderStyle: 'dashed',
  },
  dailyIcon: { fontSize: 50, marginBottom: 10 },
  dailyPoints: { color: '#FFD700', fontSize: 28, fontWeight: 'bold' },
  dailyStreak: { color: '#00FFFF', marginTop: 5 },
  streakDays: { flexDirection: 'row', marginTop: 20, gap: 8 },
  streakDay: {
    width: 36,
    height: 36,
    borderRadius: 18,
    backgroundColor: 'rgba(255,255,255,0.1)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  streakDayCompleted: { backgroundColor: '#00FF41' },
  streakDayActive: { backgroundColor: '#FF6B35' },
  streakDayText: { color: '#fff', fontWeight: 'bold' },
  claimBtn: {
    backgroundColor: '#00FF41',
    borderRadius: 15,
    paddingHorizontal: 40,
    paddingVertical: 15,
    marginTop: 20,
  },
  claimBtnDisabled: { backgroundColor: '#666' },
  claimBtnText: { color: '#000', fontWeight: 'bold', fontSize: 16 },
  achievementsGrid: { flexDirection: 'row', flexWrap: 'wrap', gap: 10 },
  achievementCard: {
    width: '48%',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 15,
    alignItems: 'center',
  },
  achievementEarned: { borderWidth: 1, borderColor: '#00FF41', backgroundColor: 'rgba(0,255,65,0.1)' },
  achievementIcon: { fontSize: 30, marginBottom: 8 },
  achievementName: { color: '#fff', fontWeight: 'bold', textAlign: 'center' },
  achievementDesc: { color: '#888', fontSize: 11, textAlign: 'center', marginTop: 4 },
  achievementPoints: { color: '#FFD700', fontWeight: 'bold', marginTop: 8 },
  leaderboardItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#1a1f2e',
    borderRadius: 12,
    padding: 12,
    marginBottom: 8,
  },
  rankBadge: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255,255,255,0.1)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  rank1: { backgroundColor: '#FFD700' },
  rank2: { backgroundColor: '#C0C0C0' },
  rank3: { backgroundColor: '#CD7F32' },
  rankText: { fontSize: 16, fontWeight: 'bold' },
  leaderInfo: { flex: 1, marginLeft: 12 },
  leaderLevel: { color: '#fff', fontWeight: 'bold' },
  leaderAddress: { color: '#888', fontSize: 12 },
  leaderPoints: { color: '#FFD700', fontWeight: 'bold' },
});
