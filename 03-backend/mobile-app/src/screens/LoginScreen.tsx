/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  LOGIN SCREEN
 * ═══════════════════════════════════════════════════════════════════════════════
 */

import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  Image,
  Alert,
} from 'react-native';
import {
  TextInput,
  Button,
  Text,
  Surface,
  ActivityIndicator,
} from 'react-native-paper';
import * as LocalAuthentication from 'expo-local-authentication';
import { useAuthStore } from '../store/authStore';
import { LinearGradient } from 'expo-linear-gradient';

export default function LoginScreen({ navigation }: any) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const { login, isLoading, error, clearError } = useAuthStore();

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert('Error', 'Please enter email and password');
      return;
    }

    const success = await login(email, password);
    
    if (!success && error) {
      Alert.alert('Login Failed', error);
      clearError();
    }
  };

  const handleBiometricLogin = async () => {
    const result = await LocalAuthentication.authenticateAsync({
      promptMessage: 'Login with Biometrics',
      fallbackLabel: 'Use Password',
    });

    if (result.success) {
      // Get stored credentials and login
      // Implementation depends on your secure storage
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
    >
      <ScrollView contentContainerStyle={styles.scrollContent}>
        <View style={styles.logoContainer}>
          <Text style={styles.logo}>🦅</Text>
          <Text style={styles.title}>IERAHKWA</Text>
          <Text style={styles.subtitle}>Sovereign Government Platform</Text>
        </View>

        <Surface style={styles.formContainer}>
          <Text style={styles.welcomeText}>Welcome Back</Text>
          
          <TextInput
            label="Email"
            value={email}
            onChangeText={setEmail}
            mode="outlined"
            keyboardType="email-address"
            autoCapitalize="none"
            style={styles.input}
            outlineColor="#333"
            activeOutlineColor="#00FF41"
            textColor="#fff"
            theme={{ colors: { onSurfaceVariant: '#888' }}}
          />

          <TextInput
            label="Password"
            value={password}
            onChangeText={setPassword}
            mode="outlined"
            secureTextEntry={!showPassword}
            style={styles.input}
            outlineColor="#333"
            activeOutlineColor="#00FF41"
            textColor="#fff"
            theme={{ colors: { onSurfaceVariant: '#888' }}}
            right={
              <TextInput.Icon
                icon={showPassword ? 'eye-off' : 'eye'}
                onPress={() => setShowPassword(!showPassword)}
                color="#888"
              />
            }
          />

          <Button
            mode="contained"
            onPress={handleLogin}
            loading={isLoading}
            disabled={isLoading}
            style={styles.loginButton}
            buttonColor="#00FF41"
            textColor="#000"
          >
            Login
          </Button>

          <Button
            mode="outlined"
            onPress={handleBiometricLogin}
            style={styles.biometricButton}
            icon="fingerprint"
            textColor="#00FF41"
          >
            Login with Biometrics
          </Button>

          <View style={styles.footer}>
            <Text style={styles.footerText}>Don't have an account? </Text>
            <Button
              mode="text"
              onPress={() => navigation.navigate('Register')}
              textColor="#00FF41"
              compact
            >
              Register
            </Button>
          </View>
        </Surface>

        <Text style={styles.version}>v1.0.0 • Sovereign Technology</Text>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#0A0A0F',
  },
  scrollContent: {
    flexGrow: 1,
    justifyContent: 'center',
    padding: 20,
  },
  logoContainer: {
    alignItems: 'center',
    marginBottom: 40,
  },
  logo: {
    fontSize: 80,
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#00FF41',
    letterSpacing: 4,
    marginTop: 10,
  },
  subtitle: {
    fontSize: 14,
    color: '#888',
    marginTop: 5,
  },
  formContainer: {
    padding: 20,
    borderRadius: 16,
    backgroundColor: '#1A1A2E',
  },
  welcomeText: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#fff',
    textAlign: 'center',
    marginBottom: 20,
  },
  input: {
    marginBottom: 15,
    backgroundColor: '#0A0A0F',
  },
  loginButton: {
    marginTop: 10,
    paddingVertical: 5,
  },
  biometricButton: {
    marginTop: 15,
    borderColor: '#00FF41',
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 20,
  },
  footerText: {
    color: '#888',
  },
  version: {
    textAlign: 'center',
    color: '#444',
    marginTop: 30,
    fontSize: 12,
  },
});
