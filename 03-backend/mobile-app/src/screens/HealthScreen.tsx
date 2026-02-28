/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  HEALTH SCREEN - Sovereign Health Records & Services
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Provides access to health records, telemedicine, prescriptions,
 *   vaccination records, insurance, and wellness tracking for sovereign citizens.
 *   All health data is encrypted and stored on the sovereign blockchain.
 *
 * @module screens/HealthScreen
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
import { useAuthStore } from '../store/authStore';

/** @typedef {Object} HealthMetric */
/** @typedef {Object} Appointment */

/**
 * HealthScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The health records and services screen
 */
export default function HealthScreen({ navigation }) {
  const { t } = useTranslation();
  const { user } = useAuthStore();
  const [refreshing, setRefreshing] = useState(false);

  const [healthMetrics] = useState([
    { label: t('health_heart_rate') || 'Heart Rate', value: '72', unit: 'BPM', icon: 'heart-pulse', color: '#FF4444', trend: 'stable' },
    { label: t('health_blood_pressure') || 'Blood Pressure', value: '120/80', unit: 'mmHg', icon: 'water', color: '#00FFFF', trend: 'stable' },
    { label: t('health_glucose') || 'Glucose', value: '95', unit: 'mg/dL', icon: 'test-tube', color: '#FFD700', trend: 'down' },
    { label: t('health_weight') || 'Weight', value: '72.5', unit: 'kg', icon: 'scale-bathroom', color: '#E040FB', trend: 'stable' },
  ]);

  const [appointments] = useState([
    { id: '1', doctor: 'Dr. Aiakwiraien', specialty: 'General Medicine', date: '2026-03-05', time: '10:00 AM', type: 'telemedicine' },
    { id: '2', doctor: 'Dr. Tekahionwake', specialty: 'Dental', date: '2026-03-12', time: '2:30 PM', type: 'in-person' },
  ]);

  const [vaccinations] = useState([
    { name: 'COVID-19 Booster', date: '2025-11-15', status: 'complete' },
    { name: 'Influenza 2026', date: '2026-01-20', status: 'complete' },
    { name: 'Tdap', date: '2026-06-01', status: 'scheduled' },
  ]);

  const onRefresh = async () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 1500);
  };

  const healthServices = [
    { icon: 'video', label: t('health_telemedicine') || 'Telemedicine', screen: 'Telemedicine', color: '#00FF41' },
    { icon: 'pill', label: t('health_prescriptions') || 'Prescriptions', screen: 'Prescriptions', color: '#00FFFF' },
    { icon: 'needle', label: t('health_vaccines') || 'Vaccines', screen: 'Vaccines', color: '#FFD700' },
    { icon: 'shield-plus', label: t('health_insurance') || 'Insurance', screen: 'HealthInsurance', color: '#E040FB' },
    { icon: 'file-document', label: t('health_records') || 'Records', screen: 'HealthRecords', color: '#FF9100' },
    { icon: 'brain', label: t('health_mental') || 'Mental Health', screen: 'MentalHealth', color: '#7C4DFF' },
  ];

  return (
    <ScrollView
      style={styles.container}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      accessibilityLabel={t('health_screen_title') || 'Health Services Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <Text style={styles.headerTitle}>{t('health_title') || 'Health & Wellness'}</Text>
        <Text style={styles.headerSubtitle}>{t('health_subtitle') || 'Sovereign Healthcare System'}</Text>
      </View>

      {/* Health Score */}
      <Surface style={styles.scoreCard} accessibilityLabel="Health wellness score">
        <View style={styles.scoreHeader}>
          <Icon name="heart" size={24} color="#FF4444" />
          <Text style={styles.scoreTitle}>{t('health_score') || 'Wellness Score'}</Text>
        </View>
        <Text style={styles.scoreValue}>87<Text style={styles.scoreMax}>/100</Text></Text>
        <ProgressBar progress={0.87} color="#00FF41" style={styles.progressBar} />
        <Text style={styles.scoreDescription}>
          {t('health_score_desc') || 'Based on your latest health metrics and activity'}
        </Text>
      </Surface>

      {/* Health Metrics Grid */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('health_metrics') || 'Health Metrics'}</Text>
        <View style={styles.metricsGrid}>
          {healthMetrics.map((metric, index) => (
            <Surface key={index} style={styles.metricCard} accessibilityLabel={`${metric.label}: ${metric.value} ${metric.unit}`}>
              <Icon name={metric.icon} size={24} color={metric.color} />
              <Text style={styles.metricValue}>{metric.value}</Text>
              <Text style={styles.metricUnit}>{metric.unit}</Text>
              <Text style={styles.metricLabel}>{metric.label}</Text>
              <View style={styles.trendBadge}>
                <Icon
                  name={metric.trend === 'up' ? 'trending-up' : metric.trend === 'down' ? 'trending-down' : 'trending-neutral'}
                  size={14}
                  color={metric.trend === 'stable' ? '#00FF41' : metric.trend === 'down' ? '#FFD700' : '#FF4444'}
                />
              </View>
            </Surface>
          ))}
        </View>
      </View>

      {/* Health Services */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('health_services') || 'Health Services'}</Text>
        <View style={styles.servicesGrid}>
          {healthServices.map((service, index) => (
            <TouchableOpacity
              key={index}
              style={styles.serviceCard}
              onPress={() => navigation.navigate(service.screen)}
              accessibilityLabel={service.label}
              accessibilityRole="button"
            >
              <View style={[styles.serviceIcon, { backgroundColor: service.color + '20' }]}>
                <Icon name={service.icon} size={24} color={service.color} />
              </View>
              <Text style={styles.serviceLabel}>{service.label}</Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>

      {/* Upcoming Appointments */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('health_appointments') || 'Upcoming Appointments'}</Text>
          <TouchableOpacity onPress={() => navigation.navigate('BookAppointment')}>
            <Text style={styles.addNew}>+ {t('health_book') || 'Book'}</Text>
          </TouchableOpacity>
        </View>
        {appointments.map((appt) => (
          <Surface key={appt.id} style={styles.appointmentCard}>
            <View style={[styles.apptTypeIcon, { backgroundColor: appt.type === 'telemedicine' ? '#00FF4120' : '#00FFFF20' }]}>
              <Icon
                name={appt.type === 'telemedicine' ? 'video' : 'hospital-building'}
                size={24}
                color={appt.type === 'telemedicine' ? '#00FF41' : '#00FFFF'}
              />
            </View>
            <View style={styles.apptInfo}>
              <Text style={styles.apptDoctor}>{appt.doctor}</Text>
              <Text style={styles.apptSpecialty}>{appt.specialty}</Text>
              <Text style={styles.apptDate}>{appt.date} at {appt.time}</Text>
            </View>
            <TouchableOpacity
              style={styles.apptAction}
              accessibilityLabel={`Join appointment with ${appt.doctor}`}
              accessibilityRole="button"
            >
              <Icon name="chevron-right" size={24} color="#888" />
            </TouchableOpacity>
          </Surface>
        ))}
      </View>

      {/* Vaccination Records */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('health_vaccinations') || 'Vaccination Records'}</Text>
        {vaccinations.map((vaccine, index) => (
          <Surface key={index} style={styles.vaccineCard}>
            <Icon
              name={vaccine.status === 'complete' ? 'check-circle' : 'clock-outline'}
              size={20}
              color={vaccine.status === 'complete' ? '#00FF41' : '#FFD700'}
            />
            <View style={styles.vaccineInfo}>
              <Text style={styles.vaccineName}>{vaccine.name}</Text>
              <Text style={styles.vaccineDate}>{vaccine.date}</Text>
            </View>
            <Text style={[styles.vaccineStatus, { color: vaccine.status === 'complete' ? '#00FF41' : '#FFD700' }]}>
              {vaccine.status.toUpperCase()}
            </Text>
          </Surface>
        ))}
      </View>

      {/* Emergency Button */}
      <Button
        mode="contained"
        onPress={() => navigation.navigate('Emergency')}
        icon="phone-alert"
        buttonColor="#FF4444"
        textColor="#fff"
        style={styles.emergencyButton}
        labelStyle={styles.emergencyLabel}
        accessibilityLabel="Emergency - Call for help"
        accessibilityRole="button"
      >
        {t('health_emergency') || 'EMERGENCY'}
      </Button>

      <View style={{ height: 30 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#888', fontSize: 14, marginTop: 4 },
  scoreCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 20, marginBottom: 20 },
  scoreHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: 10 },
  scoreTitle: { color: '#fff', fontSize: 16, fontWeight: 'bold', marginLeft: 10 },
  scoreValue: { color: '#00FF41', fontSize: 48, fontWeight: 'bold' },
  scoreMax: { color: '#888', fontSize: 24 },
  progressBar: { marginTop: 10, height: 8, borderRadius: 4, backgroundColor: '#333' },
  scoreDescription: { color: '#888', fontSize: 12, marginTop: 10 },
  section: { marginBottom: 25 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  addNew: { color: '#00FF41', fontSize: 14, fontWeight: '600' },
  metricsGrid: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  metricCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, width: '48%', alignItems: 'center', marginBottom: 12 },
  metricValue: { color: '#fff', fontSize: 28, fontWeight: 'bold', marginTop: 8 },
  metricUnit: { color: '#888', fontSize: 12 },
  metricLabel: { color: '#aaa', fontSize: 12, marginTop: 4 },
  trendBadge: { position: 'absolute', top: 8, right: 8 },
  servicesGrid: { flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' },
  serviceCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, width: '31%', alignItems: 'center', marginBottom: 12 },
  serviceIcon: { width: 48, height: 48, borderRadius: 14, justifyContent: 'center', alignItems: 'center', marginBottom: 8 },
  serviceLabel: { color: '#fff', fontSize: 11, textAlign: 'center' },
  appointmentCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 16, flexDirection: 'row', alignItems: 'center', marginBottom: 10 },
  apptTypeIcon: { width: 48, height: 48, borderRadius: 14, justifyContent: 'center', alignItems: 'center' },
  apptInfo: { flex: 1, marginLeft: 12 },
  apptDoctor: { color: '#fff', fontSize: 16, fontWeight: '600' },
  apptSpecialty: { color: '#888', fontSize: 13, marginTop: 2 },
  apptDate: { color: '#00FFFF', fontSize: 12, marginTop: 4 },
  apptAction: { padding: 8 },
  vaccineCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 14, flexDirection: 'row', alignItems: 'center', marginBottom: 8 },
  vaccineInfo: { flex: 1, marginLeft: 12 },
  vaccineName: { color: '#fff', fontSize: 14, fontWeight: '500' },
  vaccineDate: { color: '#888', fontSize: 12, marginTop: 2 },
  vaccineStatus: { fontSize: 11, fontWeight: 'bold' },
  emergencyButton: { borderRadius: 12, paddingVertical: 8, marginTop: 10 },
  emergencyLabel: { fontSize: 18, fontWeight: 'bold' },
});
