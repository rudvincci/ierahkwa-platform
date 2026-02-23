/**
 * IERAHKWA SOVEREIGN PLATFORM - MOBILE APP
 * React Native Application
 * 
 * Sovereign Government of Ierahkwa Ne Kanienke
 * © 2026 All Rights Reserved
 */

import React, { useEffect, useState } from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { StatusBar, useColorScheme } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { I18nextProvider } from 'react-i18next';
import i18n from './src/i18n';

// Screens
import DashboardScreen from './src/screens/DashboardScreen';
import WalletScreen from './src/screens/WalletScreen';
import TradeScreen from './src/screens/TradeScreen';
import GovernanceScreen from './src/screens/GovernanceScreen';
import RewardsScreen from './src/screens/RewardsScreen';
import SettingsScreen from './src/screens/SettingsScreen';
import TokenDetailScreen from './src/screens/TokenDetailScreen';
import BridgeScreen from './src/screens/BridgeScreen';

const Tab = createBottomTabNavigator();
const Stack = createNativeStackNavigator();

// Tab Navigator
function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={{
        tabBarStyle: {
          backgroundColor: '#0a0e17',
          borderTopColor: '#1a1f2e',
          paddingBottom: 5,
          height: 60,
        },
        tabBarActiveTintColor: '#FFD700',
        tabBarInactiveTintColor: '#666',
        headerStyle: {
          backgroundColor: '#0a0e17',
        },
        headerTintColor: '#FFD700',
      }}
    >
      <Tab.Screen 
        name="Dashboard" 
        component={DashboardScreen}
        options={{
          tabBarIcon: ({ color }) => <TabIcon name="home" color={color} />,
          headerTitle: 'IERAHKWA',
        }}
      />
      <Tab.Screen 
        name="Wallet" 
        component={WalletScreen}
        options={{
          tabBarIcon: ({ color }) => <TabIcon name="wallet" color={color} />,
        }}
      />
      <Tab.Screen 
        name="Trade" 
        component={TradeScreen}
        options={{
          tabBarIcon: ({ color }) => <TabIcon name="swap" color={color} />,
        }}
      />
      <Tab.Screen 
        name="Governance" 
        component={GovernanceScreen}
        options={{
          tabBarIcon: ({ color }) => <TabIcon name="vote" color={color} />,
        }}
      />
      <Tab.Screen 
        name="Rewards" 
        component={RewardsScreen}
        options={{
          tabBarIcon: ({ color }) => <TabIcon name="trophy" color={color} />,
        }}
      />
    </Tab.Navigator>
  );
}

// Tab Icon Component
function TabIcon({ name, color }) {
  const icons = {
    home: '🏛️',
    wallet: '💰',
    swap: '💱',
    vote: '🗳️',
    trophy: '🏆',
  };
  
  return (
    <Text style={{ fontSize: 24 }}>{icons[name]}</Text>
  );
}

// Main App
export default function App() {
  const [isLoading, setIsLoading] = useState(true);
  const [userToken, setUserToken] = useState(null);
  const isDarkMode = useColorScheme() === 'dark';

  useEffect(() => {
    // Check for stored credentials
    AsyncStorage.getItem('userToken').then(token => {
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
            primary: '#FFD700',
            background: '#0a0e17',
            card: '#1a1f2e',
            text: '#ffffff',
            border: '#2a2f3e',
            notification: '#FF6B35',
          },
        }}
      >
        <StatusBar barStyle="light-content" backgroundColor="#0a0e17" />
        <Stack.Navigator
          screenOptions={{
            headerStyle: { backgroundColor: '#0a0e17' },
            headerTintColor: '#FFD700',
          }}
        >
          <Stack.Screen 
            name="Main" 
            component={MainTabs}
            options={{ headerShown: false }}
          />
          <Stack.Screen name="TokenDetail" component={TokenDetailScreen} />
          <Stack.Screen name="Bridge" component={BridgeScreen} />
          <Stack.Screen name="Settings" component={SettingsScreen} />
        </Stack.Navigator>
      </NavigationContainer>
    </I18nextProvider>
  );
}

// Splash Screen
function SplashScreen() {
  return (
    <View style={{ flex: 1, backgroundColor: '#0a0e17', justifyContent: 'center', alignItems: 'center' }}>
      <Text style={{ fontSize: 60 }}>🏛️</Text>
      <Text style={{ color: '#FFD700', fontSize: 24, marginTop: 20, fontWeight: 'bold' }}>
        IERAHKWA
      </Text>
      <Text style={{ color: '#888', marginTop: 10 }}>
        Sovereign Digital Ecosystem
      </Text>
    </View>
  );
}

import { View, Text } from 'react-native';
