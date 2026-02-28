/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  IERAHKWA SOVEREIGN PLATFORM - MOBILE APP
 *  React Native Application with Bottom Tab Navigation
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Main application entry point with bottom tab navigation
 *   integrating all sovereign services: Home, Wallet, Government, Health,
 *   Education, Security, Marketplace, Communication, Profile, and Settings.
 *
 * @version 3.9.0
 * @copyright 2026 Sovereign Government of Ierahkwa Ne Kanienke
 */

import React, { useEffect, useState } from 'react';
import { View, Text, StatusBar, useColorScheme } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { I18nextProvider } from 'react-i18next';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import i18n from './src/i18n';

// ─── Screen Imports ──────────────────────────────────────────────────────────
import HomeScreen from './src/screens/HomeScreen';
import WalletScreen from './src/screens/WalletScreen';
import GovernmentScreen from './src/screens/GovernmentScreen';
import HealthScreen from './src/screens/HealthScreen';
import EducationScreen from './src/screens/EducationScreen';
import SecurityScreen from './src/screens/SecurityScreen';
import MarketplaceScreen from './src/screens/MarketplaceScreen';
import CommunicationScreen from './src/screens/CommunicationScreen';
import ProfileScreen from './src/screens/ProfileScreen';
import SettingsScreen from './src/screens/SettingsScreen';
import LoginScreen from './src/screens/LoginScreen';
import AIAssistantScreen from './src/screens/AIAssistantScreen';
import DashboardScreen from './src/screens/DashboardScreen';
import GovernanceScreen from './src/screens/GovernanceScreen';
import TradeScreen from './src/screens/TradeScreen';
import BridgeScreen from './src/screens/BridgeScreen';
import RewardsScreen from './src/screens/RewardsScreen';

// ─── Navigation Setup ────────────────────────────────────────────────────────
const Tab = createBottomTabNavigator();
const Stack = createNativeStackNavigator();

/**
 * Tab icon mapping for bottom navigation
 * @param {string} routeName - The route name
 * @param {boolean} focused - Whether the tab is focused
 * @returns {string} MaterialCommunityIcons icon name
 */
const getTabIcon = (routeName, focused) => {
  const icons = {
    Home: focused ? 'home' : 'home-outline',
    Wallet: focused ? 'wallet' : 'wallet-outline',
    Government: focused ? 'bank' : 'bank-outline',
    Health: focused ? 'heart-pulse' : 'heart-outline',
    More: focused ? 'dots-horizontal-circle' : 'dots-horizontal-circle-outline',
  };
  return icons[routeName] || 'circle';
};

/**
 * Main bottom tab navigator with 5 primary tabs
 * Additional screens accessible through the "More" tab or stack navigation
 */
function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => (
          <Icon name={getTabIcon(route.name, focused)} size={24} color={color} />
        ),
        tabBarStyle: {
          backgroundColor: '#0A0A0F',
          borderTopColor: '#1A1A2E',
          borderTopWidth: 1,
          paddingBottom: 8,
          paddingTop: 8,
          height: 64,
        },
        tabBarActiveTintColor: '#00FF41',
        tabBarInactiveTintColor: '#666',
        headerStyle: {
          backgroundColor: '#0A0A0F',
          elevation: 0,
          shadowOpacity: 0,
          borderBottomWidth: 1,
          borderBottomColor: '#1A1A2E',
        },
        headerTintColor: '#fff',
        headerTitleStyle: {
          fontWeight: 'bold',
        },
      })}
    >
      <Tab.Screen
        name="Home"
        component={HomeScreen}
        options={{
          headerTitle: 'IERAHKWA',
          headerTitleStyle: { color: '#00FF41', fontWeight: 'bold', fontSize: 20 },
          tabBarAccessibilityLabel: 'Home dashboard',
        }}
      />
      <Tab.Screen
        name="Wallet"
        component={WalletScreen}
        options={{
          headerTitle: 'Sovereign Wallet',
          tabBarAccessibilityLabel: 'BDET Wallet',
        }}
      />
      <Tab.Screen
        name="Government"
        component={GovernmentScreen}
        options={{
          headerTitle: 'Citizen Services',
          tabBarAccessibilityLabel: 'Government citizen services',
        }}
      />
      <Tab.Screen
        name="Health"
        component={HealthScreen}
        options={{
          headerTitle: 'Health & Wellness',
          tabBarAccessibilityLabel: 'Health records and services',
        }}
      />
      <Tab.Screen
        name="More"
        component={MoreScreen}
        options={{
          headerTitle: 'More Services',
          tabBarAccessibilityLabel: 'More sovereign services',
        }}
      />
    </Tab.Navigator>
  );
}

/**
 * "More" screen that provides access to additional services
 * not shown in the primary bottom tabs
 */
function MoreScreen({ navigation }) {
  const moreItems = [
    { icon: 'school', label: 'Education', screen: 'Education', color: '#9C27B0', desc: 'Learning portal & courses' },
    { icon: 'shield-lock', label: 'Security', screen: 'Security', color: '#f44336', desc: 'AI agents & cyber defense' },
    { icon: 'store', label: 'Marketplace', screen: 'Marketplace', color: '#FF9100', desc: 'Sovereign commerce' },
    { icon: 'message-text', label: 'Messages', screen: 'Communication', color: '#E040FB', desc: 'Encrypted messaging' },
    { icon: 'swap-horizontal', label: 'Trading', screen: 'Trade', color: '#00FFFF', desc: 'Token exchange' },
    { icon: 'vote', label: 'Governance', screen: 'Governance', color: '#1565C0', desc: 'Proposals & voting' },
    { icon: 'bridge', label: 'Bridge', screen: 'Bridge', color: '#43A047', desc: 'Cross-chain transfers' },
    { icon: 'trophy', label: 'Rewards', screen: 'Rewards', color: '#FFD700', desc: 'Earn & claim rewards' },
    { icon: 'robot', label: 'AI Assistant', screen: 'AIAssistant', color: '#7C4DFF', desc: 'Sovereign AI helper' },
    { icon: 'account', label: 'Profile', screen: 'Profile', color: '#00FF41', desc: 'Identity & settings' },
    { icon: 'cog', label: 'Settings', screen: 'Settings', color: '#888', desc: 'Language, theme, notifications' },
  ];

  return (
    <View style={{ flex: 1, backgroundColor: '#0A0A0F', padding: 16 }}>
      <View style={{ flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-between' }}>
        {moreItems.map((item, index) => (
          <View
            key={index}
            style={{
              backgroundColor: '#1A1A2E',
              borderRadius: 12,
              padding: 16,
              width: '48%',
              marginBottom: 12,
            }}
            accessible={true}
            accessibilityLabel={`${item.label}: ${item.desc}`}
            accessibilityRole="button"
          >
            <View
              style={{
                width: 48, height: 48, borderRadius: 14,
                backgroundColor: item.color + '20',
                justifyContent: 'center', alignItems: 'center', marginBottom: 10,
              }}
            >
              <Icon
                name={item.icon}
                size={24}
                color={item.color}
                onPress={() => navigation.navigate(item.screen)}
              />
            </View>
            <Text
              style={{ color: '#fff', fontSize: 15, fontWeight: '600' }}
              onPress={() => navigation.navigate(item.screen)}
            >
              {item.label}
            </Text>
            <Text style={{ color: '#888', fontSize: 11, marginTop: 4 }}>{item.desc}</Text>
          </View>
        ))}
      </View>
    </View>
  );
}

/**
 * Splash screen shown during app initialization
 */
function SplashScreen() {
  return (
    <View
      style={{
        flex: 1,
        backgroundColor: '#0A0A0F',
        justifyContent: 'center',
        alignItems: 'center',
      }}
      accessibilityLabel="Loading Ierahkwa Sovereign Platform"
    >
      <Icon name="shield-star" size={80} color="#00FF41" />
      <Text style={{ color: '#00FF41', fontSize: 28, marginTop: 20, fontWeight: 'bold' }}>
        IERAHKWA
      </Text>
      <Text style={{ color: '#FFD700', fontSize: 14, marginTop: 8 }}>
        Ne Kanienke - Sovereign Digital Nation
      </Text>
      <Text style={{ color: '#888', marginTop: 20, fontSize: 12 }}>
        Loading sovereign services...
      </Text>
    </View>
  );
}

/**
 * Main App component
 * Wraps the navigation container with i18n provider
 * and handles authentication state
 */
export default function App() {
  const [isLoading, setIsLoading] = useState(true);
  const [userToken, setUserToken] = useState(null);

  useEffect(() => {
    AsyncStorage.getItem('userToken').then((token) => {
      setUserToken(token);
      setIsLoading(false);
    });
  }, []);

  if (isLoading) {
    return <SplashScreen />;
  }

  return (
    <I18nextProvider i18n={i18n}>
      <NavigationContainer
        theme={{
          dark: true,
          colors: {
            primary: '#00FF41',
            background: '#0A0A0F',
            card: '#1A1A2E',
            text: '#ffffff',
            border: '#2A2A3E',
            notification: '#FF4444',
          },
        }}
      >
        <StatusBar barStyle="light-content" backgroundColor="#0A0A0F" />
        <Stack.Navigator
          screenOptions={{
            headerStyle: { backgroundColor: '#0A0A0F' },
            headerTintColor: '#fff',
            headerTitleStyle: { fontWeight: 'bold' },
          }}
        >
          {userToken == null ? (
            <Stack.Screen
              name="Login"
              component={LoginScreen}
              options={{ headerShown: false }}
            />
          ) : null}
          <Stack.Screen
            name="Main"
            component={MainTabs}
            options={{ headerShown: false }}
          />
          {/* Additional stack screens accessible from any tab */}
          <Stack.Screen name="Education" component={EducationScreen} options={{ headerTitle: 'Learning Portal' }} />
          <Stack.Screen name="Security" component={SecurityScreen} options={{ headerTitle: 'Security Center' }} />
          <Stack.Screen name="Marketplace" component={MarketplaceScreen} options={{ headerTitle: 'Marketplace' }} />
          <Stack.Screen name="Communication" component={CommunicationScreen} options={{ headerTitle: 'Messages' }} />
          <Stack.Screen name="Profile" component={ProfileScreen} options={{ headerTitle: 'Profile' }} />
          <Stack.Screen name="Settings" component={SettingsScreen} options={{ headerTitle: 'Settings' }} />
          <Stack.Screen name="Trade" component={TradeScreen} options={{ headerTitle: 'Trading' }} />
          <Stack.Screen name="Governance" component={GovernanceScreen} options={{ headerTitle: 'Governance' }} />
          <Stack.Screen name="Bridge" component={BridgeScreen} options={{ headerTitle: 'Bridge' }} />
          <Stack.Screen name="Rewards" component={RewardsScreen} options={{ headerTitle: 'Rewards' }} />
          <Stack.Screen name="AIAssistant" component={AIAssistantScreen} options={{ headerTitle: 'AI Assistant' }} />
          <Stack.Screen name="Dashboard" component={DashboardScreen} options={{ headerTitle: 'Dashboard' }} />
        </Stack.Navigator>
      </NavigationContainer>
    </I18nextProvider>
  );
}
