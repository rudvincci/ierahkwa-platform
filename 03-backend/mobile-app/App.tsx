/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  IERAHKWA MOBILE APP - React Native / Expo
 *  Complete Mobile Banking & Government Services
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import React, { useEffect, useState } from 'react';
import { StatusBar } from 'expo-status-bar';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Provider as PaperProvider, DefaultTheme } from 'react-native-paper';
import * as SecureStore from 'expo-secure-store';
import * as LocalAuthentication from 'expo-local-authentication';
import { QueryClient, QueryClientProvider } from 'react-query';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

// Screens
import LoginScreen from './src/screens/LoginScreen';
import RegisterScreen from './src/screens/RegisterScreen';
import HomeScreen from './src/screens/HomeScreen';
import WalletScreen from './src/screens/WalletScreen';
import SendScreen from './src/screens/SendScreen';
import ReceiveScreen from './src/screens/ReceiveScreen';
import TransactionsScreen from './src/screens/TransactionsScreen';
import ServicesScreen from './src/screens/ServicesScreen';
import GovernmentScreen from './src/screens/GovernmentScreen';
import ProfileScreen from './src/screens/ProfileScreen';
import SettingsScreen from './src/screens/SettingsScreen';
import ScanQRScreen from './src/screens/ScanQRScreen';
import AIAssistantScreen from './src/screens/AIAssistantScreen';

// Store
import { useAuthStore } from './src/store/authStore';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();
const queryClient = new QueryClient();

// ═══════════════════════════════════════════════════════════════════════════════
//  THEME
// ═══════════════════════════════════════════════════════════════════════════════

const theme = {
  ...DefaultTheme,
  colors: {
    ...DefaultTheme.colors,
    primary: '#00FF41',
    secondary: '#FFD700',
    accent: '#00FFFF',
    background: '#0A0A0F',
    surface: '#1A1A2E',
    text: '#FFFFFF',
    error: '#FF4444',
  },
  dark: true,
};

// ═══════════════════════════════════════════════════════════════════════════════
//  MAIN TAB NAVIGATOR
// ═══════════════════════════════════════════════════════════════════════════════

function MainTabs() {
  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName;
          switch (route.name) {
            case 'Home':
              iconName = focused ? 'home' : 'home-outline';
              break;
            case 'Wallet':
              iconName = focused ? 'wallet' : 'wallet-outline';
              break;
            case 'Services':
              iconName = focused ? 'apps' : 'apps-box';
              break;
            case 'Government':
              iconName = focused ? 'bank' : 'bank-outline';
              break;
            case 'Profile':
              iconName = focused ? 'account' : 'account-outline';
              break;
            default:
              iconName = 'circle';
          }
          return <Icon name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: '#00FF41',
        tabBarInactiveTintColor: '#666',
        tabBarStyle: {
          backgroundColor: '#0A0A0F',
          borderTopColor: '#1A1A2E',
          height: 60,
          paddingBottom: 8,
        },
        headerStyle: {
          backgroundColor: '#0A0A0F',
        },
        headerTintColor: '#fff',
      })}
    >
      <Tab.Screen name="Home" component={HomeScreen} />
      <Tab.Screen name="Wallet" component={WalletScreen} />
      <Tab.Screen name="Services" component={ServicesScreen} />
      <Tab.Screen name="Government" component={GovernmentScreen} />
      <Tab.Screen name="Profile" component={ProfileScreen} />
    </Tab.Navigator>
  );
}

// ═══════════════════════════════════════════════════════════════════════════════
//  APP COMPONENT
// ═══════════════════════════════════════════════════════════════════════════════

export default function App() {
  const [isLoading, setIsLoading] = useState(true);
  const { isAuthenticated, checkAuth } = useAuthStore();

  useEffect(() => {
    initializeApp();
  }, []);

  const initializeApp = async () => {
    try {
      // Check biometric availability
      const compatible = await LocalAuthentication.hasHardwareAsync();
      const enrolled = await LocalAuthentication.isEnrolledAsync();
      
      // Check stored auth token
      await checkAuth();
      
      setIsLoading(false);
    } catch (error) {
      console.error('App initialization error:', error);
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return null; // Or splash screen
  }

  return (
    <QueryClientProvider client={queryClient}>
      <PaperProvider theme={theme}>
        <NavigationContainer>
          <StatusBar style="light" />
          <Stack.Navigator
            screenOptions={{
              headerStyle: { backgroundColor: '#0A0A0F' },
              headerTintColor: '#fff',
              contentStyle: { backgroundColor: '#0A0A0F' },
            }}
          >
            {!isAuthenticated ? (
              // Auth Stack
              <>
                <Stack.Screen 
                  name="Login" 
                  component={LoginScreen}
                  options={{ headerShown: false }}
                />
                <Stack.Screen 
                  name="Register" 
                  component={RegisterScreen}
                  options={{ title: 'Create Account' }}
                />
              </>
            ) : (
              // Main App Stack
              <>
                <Stack.Screen 
                  name="Main" 
                  component={MainTabs}
                  options={{ headerShown: false }}
                />
                <Stack.Screen name="Send" component={SendScreen} />
                <Stack.Screen name="Receive" component={ReceiveScreen} />
                <Stack.Screen name="Transactions" component={TransactionsScreen} />
                <Stack.Screen name="ScanQR" component={ScanQRScreen} />
                <Stack.Screen name="Settings" component={SettingsScreen} />
                <Stack.Screen 
                  name="AIAssistant" 
                  component={AIAssistantScreen}
                  options={{ title: 'AI Assistant' }}
                />
              </>
            )}
          </Stack.Navigator>
        </NavigationContainer>
      </PaperProvider>
    </QueryClientProvider>
  );
}
