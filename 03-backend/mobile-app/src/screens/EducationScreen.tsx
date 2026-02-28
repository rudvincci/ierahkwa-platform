/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  EDUCATION SCREEN - Sovereign Learning Portal
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Provides access to sovereign education services including
 *   courses, certifications, language learning (Mohawk/Taino), library,
 *   and skill development programs.
 *
 * @module screens/EducationScreen
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
import { Text, Surface, Button, ProgressBar, Chip } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../store/authStore';

/**
 * EducationScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The education and learning portal screen
 */
export default function EducationScreen({ navigation }) {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [refreshing, setRefreshing] = useState(false);
  const [activeTab, setActiveTab] = useState('courses');

  const onRefresh = async () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 1500);
  };

  const [enrolledCourses] = useState([
    { id: '1', title: "Kanien'keha Language I", progress: 0.65, lessons: 24, completed: 16, instructor: 'Karakwine', category: 'language', color: '#00FF41' },
    { id: '2', title: 'Sovereign Blockchain Dev', progress: 0.30, lessons: 40, completed: 12, instructor: 'Tekahionwake', category: 'tech', color: '#00FFFF' },
    { id: '3', title: 'Taino History & Culture', progress: 0.85, lessons: 18, completed: 15, instructor: 'Cacike Agueybana', category: 'culture', color: '#FFD700' },
  ]);

  const [featuredCourses] = useState([
    { id: '10', title: 'Quantum Computing Basics', students: 1250, rating: 4.8, icon: 'atom', color: '#7C4DFF' },
    { id: '11', title: 'Indigenous Governance', students: 890, rating: 4.9, icon: 'gavel', color: '#FF9100' },
    { id: '12', title: 'Digital Sovereignty', students: 2100, rating: 4.7, icon: 'shield-lock', color: '#E040FB' },
    { id: '13', title: 'Traditional Medicine', students: 670, rating: 4.6, icon: 'flower-tulip', color: '#43A047' },
  ]);

  const categories = [
    { key: 'courses', label: t('edu_courses') || 'Courses', icon: 'book-open-variant' },
    { key: 'languages', label: t('edu_languages') || 'Languages', icon: 'translate' },
    { key: 'certifications', label: t('edu_certs') || 'Certifications', icon: 'certificate' },
    { key: 'library', label: t('edu_library') || 'Library', icon: 'library' },
  ];

  const [achievements] = useState([
    { title: 'First Lesson', icon: 'star', earned: true },
    { title: '10 Day Streak', icon: 'fire', earned: true },
    { title: 'Language Master', icon: 'translate', earned: false },
    { title: 'Certified Dev', icon: 'code-tags', earned: false },
  ]);

  return (
    <ScrollView
      style={styles.container}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      accessibilityLabel={t('edu_screen_title') || 'Education Portal Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>{t('edu_title') || 'Learning Portal'}</Text>
        <Text style={styles.headerSubtitle}>{t('edu_subtitle') || 'Ierahkwa Sovereign Academy'}</Text>
      </View>

      {/* Learning Stats */}
      <Surface style={styles.statsCard} accessibilityLabel="Your learning statistics">
        <View style={styles.statsRow}>
          <View style={styles.statItem}>
            <Text style={styles.statValue}>43</Text>
            <Text style={styles.statLabel}>{t('edu_lessons') || 'Lessons'}</Text>
          </View>
          <View style={styles.statDivider} />
          <View style={styles.statItem}>
            <Text style={styles.statValue}>12</Text>
            <Text style={styles.statLabel}>{t('edu_streak') || 'Day Streak'}</Text>
          </View>
          <View style={styles.statDivider} />
          <View style={styles.statItem}>
            <Text style={styles.statValue}>3</Text>
            <Text style={styles.statLabel}>{t('edu_enrolled') || 'Enrolled'}</Text>
          </View>
          <View style={styles.statDivider} />
          <View style={styles.statItem}>
            <Text style={styles.statValue}>850</Text>
            <Text style={styles.statLabel}>{t('edu_xp') || 'XP'}</Text>
          </View>
        </View>
      </Surface>

      {/* Category Tabs */}
      <ScrollView horizontal showsHorizontalScrollIndicator={false} style={styles.tabsScroll}>
        {categories.map((cat) => (
          <Chip
            key={cat.key}
            selected={activeTab === cat.key}
            onPress={() => setActiveTab(cat.key)}
            style={[styles.tabChip, activeTab === cat.key && styles.tabChipActive]}
            textStyle={[styles.tabText, activeTab === cat.key && styles.tabTextActive]}
            icon={() => <Icon name={cat.icon} size={18} color={activeTab === cat.key ? '#000' : '#888'} />}
            accessibilityLabel={cat.label}
            accessibilityRole="tab"
            accessibilityState={{ selected: activeTab === cat.key }}
          >
            {cat.label}
          </Chip>
        ))}
      </ScrollView>

      {/* Enrolled Courses */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('edu_my_courses') || 'My Courses'}</Text>
        {enrolledCourses.map((course) => (
          <TouchableOpacity
            key={course.id}
            onPress={() => navigation.navigate('CourseDetail', { courseId: course.id })}
            accessibilityLabel={`${course.title}, ${Math.round(course.progress * 100)}% complete`}
            accessibilityRole="button"
          >
            <Surface style={styles.courseCard}>
              <View style={styles.courseHeader}>
                <View style={[styles.courseCategory, { backgroundColor: course.color + '20' }]}>
                  <Text style={[styles.courseCategoryText, { color: course.color }]}>
                    {course.category.toUpperCase()}
                  </Text>
                </View>
                <Text style={styles.courseProgress}>{Math.round(course.progress * 100)}%</Text>
              </View>
              <Text style={styles.courseTitle}>{course.title}</Text>
              <Text style={styles.courseInstructor}>{course.instructor}</Text>
              <ProgressBar progress={course.progress} color={course.color} style={styles.courseProgressBar} />
              <Text style={styles.courseLessons}>
                {course.completed}/{course.lessons} {t('edu_lessons_done') || 'lessons completed'}
              </Text>
            </Surface>
          </TouchableOpacity>
        ))}
      </View>

      {/* Featured Courses */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('edu_featured') || 'Featured Courses'}</Text>
          <TouchableOpacity onPress={() => navigation.navigate('CoursesCatalog')}>
            <Text style={styles.viewAll}>{t('view_all') || 'View All'}</Text>
          </TouchableOpacity>
        </View>
        <ScrollView horizontal showsHorizontalScrollIndicator={false}>
          {featuredCourses.map((course) => (
            <TouchableOpacity
              key={course.id}
              onPress={() => navigation.navigate('CourseDetail', { courseId: course.id })}
              accessibilityLabel={`${course.title}, ${course.students} students, ${course.rating} rating`}
              accessibilityRole="button"
            >
              <Surface style={styles.featuredCard}>
                <View style={[styles.featuredIcon, { backgroundColor: course.color + '20' }]}>
                  <Icon name={course.icon} size={32} color={course.color} />
                </View>
                <Text style={styles.featuredTitle}>{course.title}</Text>
                <View style={styles.featuredMeta}>
                  <Icon name="account-group" size={14} color="#888" />
                  <Text style={styles.featuredStudents}>{course.students.toLocaleString()}</Text>
                  <Icon name="star" size={14} color="#FFD700" />
                  <Text style={styles.featuredRating}>{course.rating}</Text>
                </View>
              </Surface>
            </TouchableOpacity>
          ))}
        </ScrollView>
      </View>

      {/* Achievements */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('edu_achievements') || 'Achievements'}</Text>
        <View style={styles.achievementsGrid}>
          {achievements.map((achievement, index) => (
            <Surface
              key={index}
              style={[styles.achievementCard, !achievement.earned && styles.achievementLocked]}
              accessibilityLabel={`${achievement.title} - ${achievement.earned ? 'earned' : 'locked'}`}
            >
              <Icon
                name={achievement.icon}
                size={28}
                color={achievement.earned ? '#FFD700' : '#444'}
              />
              <Text style={[styles.achievementTitle, !achievement.earned && { color: '#444' }]}>
                {achievement.title}
              </Text>
            </Surface>
          ))}
        </View>
      </View>

      <View style={{ height: 30 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#888', fontSize: 14, marginTop: 4 },
  statsCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 20, marginBottom: 20 },
  statsRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  statItem: { alignItems: 'center', flex: 1 },
  statValue: { color: '#00FF41', fontSize: 24, fontWeight: 'bold' },
  statLabel: { color: '#888', fontSize: 11, marginTop: 4 },
  statDivider: { width: 1, height: 30, backgroundColor: '#333' },
  tabsScroll: { marginBottom: 20 },
  tabChip: { marginRight: 8, backgroundColor: '#1A1A2E' },
  tabChipActive: { backgroundColor: '#00FF41' },
  tabText: { color: '#888' },
  tabTextActive: { color: '#000', fontWeight: 'bold' },
  section: { marginBottom: 25 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  viewAll: { color: '#00FF41', fontSize: 14 },
  courseCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, marginBottom: 12 },
  courseHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
  courseCategory: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 8 },
  courseCategoryText: { fontSize: 10, fontWeight: 'bold' },
  courseProgress: { color: '#00FF41', fontSize: 16, fontWeight: 'bold' },
  courseTitle: { color: '#fff', fontSize: 17, fontWeight: '600', marginBottom: 4 },
  courseInstructor: { color: '#888', fontSize: 13, marginBottom: 10 },
  courseProgressBar: { height: 6, borderRadius: 3, backgroundColor: '#333' },
  courseLessons: { color: '#888', fontSize: 12, marginTop: 8 },
  featuredCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, marginRight: 12, width: 160 },
  featuredIcon: { width: 56, height: 56, borderRadius: 16, justifyContent: 'center', alignItems: 'center', marginBottom: 10 },
  featuredTitle: { color: '#fff', fontSize: 14, fontWeight: '600', marginBottom: 8 },
  featuredMeta: { flexDirection: 'row', alignItems: 'center' },
  featuredStudents: { color: '#888', fontSize: 12, marginLeft: 4, marginRight: 8 },
  featuredRating: { color: '#FFD700', fontSize: 12, marginLeft: 4 },
  achievementsGrid: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  achievementCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, width: '48%', alignItems: 'center', marginBottom: 12 },
  achievementLocked: { opacity: 0.5 },
  achievementTitle: { color: '#fff', fontSize: 12, marginTop: 8, textAlign: 'center' },
});
