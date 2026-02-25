import React, { useState, useEffect } from 'react';
import {
  StyleSheet, View, Text, ScrollView, TouchableOpacity,
  StatusBar, TextInput, ActivityIndicator, Dimensions
} from 'react-native';
import { WebView } from 'react-native-webview';
import { NavigationContainer } from '@react-navigation/native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

// ===== CONSTANTS =====
const API_URL = 'https://api.ierahkwa.org';
const COLORS = {
  bg: '#09090d', bg2: '#111116', bg3: '#1a1a20', bg4: '#232330',
  gold: '#d4a853', txt: '#e8e4df', txt2: '#8a8694', brd: '#2a2a36',
  green: '#34d399', red: '#ff4d6a', blue: '#60a5fa',
};
const NEXUS = [
  { id: 'orbital', name: 'Orbital', icon: '🛰', color: '#00bcd4', desc: 'Science & Technology' },
  { id: 'escudo', name: 'Escudo', icon: '🛡', color: '#f44336', desc: 'Defense & Security' },
  { id: 'cerebro', name: 'Cerebro', icon: '🧠', color: '#7c4dff', desc: 'Education & Research' },
  { id: 'tesoro', name: 'Tesoro', icon: '💰', color: '#ffd600', desc: 'Economy & Finance' },
  { id: 'voces', name: 'Voces', icon: '📡', color: '#e040fb', desc: 'Culture & Communication' },
  { id: 'consejo', name: 'Consejo', icon: '⚖', color: '#1565c0', desc: 'Governance & Law' },
  { id: 'tierra', name: 'Tierra', icon: '🌿', color: '#43a047', desc: 'Environment & Resources' },
  { id: 'forja', name: 'Forja', icon: '🔧', color: '#00e676', desc: 'Technology & Innovation' },
  { id: 'urbe', name: 'Urbe', icon: '🏙', color: '#ff9100', desc: 'Infrastructure & Urban' },
  { id: 'raices', name: 'Raíces', icon: '🏺', color: '#d4a853', desc: 'Identity & Heritage' },
];

// ===== API CLIENT =====
class IerahkwaAPI {
  static token: string | null = null;
  static tenant: string = 'ierahkwa-global';

  static async request(method: string, path: string, body?: any) {
    const headers: any = { 'Content-Type': 'application/json' };
    if (this.token) headers['Authorization'] = `Bearer ${this.token}`;
    headers['X-Tenant-Id'] = this.tenant;
    const res = await fetch(`${API_URL}${path}`, {
      method, headers, body: body ? JSON.stringify(body) : undefined
    });
    if (!res.ok) throw new Error(`${res.status}`);
    return res.json();
  }

  static async login(userId: string, tenantId: string, tier: string) {
    const data = await this.request('POST', '/auth/login', {
      userId, tenantId, tier, roles: ['user']
    });
    this.token = data.token;
    this.tenant = tenantId;
    return data;
  }

  static async health() { return this.request('GET', '/health'); }
  static async me() { return this.request('GET', '/auth/me'); }
  static async services() { return this.request('GET', '/services'); }
}

// ===== SCREENS =====

// Login Screen
function LoginScreen({ navigation }: any) {
  const [userId, setUserId] = useState('');
  const [tenantId, setTenantId] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setLoading(true);
    try {
      await IerahkwaAPI.login(userId || 'demo-user', tenantId || 'navajo', 'citizen');
      navigation.replace('Main');
    } catch (e) {
      alert('Login failed. Check your FWID credentials.');
    }
    setLoading(false);
  };

  return (
    <View style={s.container}>
      <StatusBar barStyle="light-content" />
      <View style={s.loginBox}>
        <Text style={s.logoText}>IERAHKWA</Text>
        <Text style={s.subtitle}>Sovereign Digital Platform</Text>
        <Text style={s.label}>FWID (User ID)</Text>
        <TextInput style={s.input} value={userId} onChangeText={setUserId}
          placeholder="Enter your FWID" placeholderTextColor={COLORS.txt2} />
        <Text style={s.label}>Tenant</Text>
        <TextInput style={s.input} value={tenantId} onChangeText={setTenantId}
          placeholder="e.g. navajo, cherokee, maya" placeholderTextColor={COLORS.txt2} />
        <TouchableOpacity style={s.btn} onPress={handleLogin} disabled={loading}>
          {loading ? <ActivityIndicator color={COLORS.bg} /> : <Text style={s.btnText}>Sign In</Text>}
        </TouchableOpacity>
        <Text style={s.tierInfo}>190 Platforms • 83 Microservices • 10 NEXUS Domains</Text>
      </View>
    </View>
  );
}

// Home / NEXUS Dashboard
function HomeScreen({ navigation }: any) {
  return (
    <ScrollView style={s.container} contentContainerStyle={{ padding: 16 }}>
      <Text style={s.heading}>NEXUS Domains</Text>
      <Text style={s.subheading}>10 interconnected sovereign systems</Text>
      {NEXUS.map(n => (
        <TouchableOpacity key={n.id} style={[s.nexusCard, { borderLeftColor: n.color }]}
          onPress={() => navigation.navigate('NexusDetail', { nexus: n })}>
          <Text style={{ fontSize: 28 }}>{n.icon}</Text>
          <View style={{ flex: 1 }}>
            <Text style={[s.nexusName, { color: n.color }]}>NEXUS {n.name}</Text>
            <Text style={s.nexusDesc}>{n.desc}</Text>
          </View>
          <Text style={s.arrow}>›</Text>
        </TouchableOpacity>
      ))}
    </ScrollView>
  );
}

// NEXUS Detail Screen
function NexusDetailScreen({ route }: any) {
  const { nexus } = route.params;
  return (
    <ScrollView style={s.container} contentContainerStyle={{ padding: 16 }}>
      <Text style={{ fontSize: 48, textAlign: 'center', marginBottom: 8 }}>{nexus.icon}</Text>
      <Text style={[s.heading, { color: nexus.color, textAlign: 'center' }]}>NEXUS {nexus.name}</Text>
      <Text style={[s.subheading, { textAlign: 'center' }]}>{nexus.desc}</Text>
      <View style={s.card}>
        <Text style={s.cardLabel}>Status</Text>
        <Text style={[s.cardValue, { color: COLORS.green }]}>All Services Online</Text>
      </View>
      <View style={s.card}>
        <Text style={s.cardLabel}>Platforms</Text>
        <Text style={s.cardValue}>Access via browser or webview</Text>
      </View>
    </ScrollView>
  );
}

// Platforms Screen (WebView shell)
function PlatformsScreen() {
  return (
    <WebView source={{ uri: 'https://ierahkwa.org/portal-central/' }}
      style={{ flex: 1, backgroundColor: COLORS.bg }}
      javaScriptEnabled={true} domStorageEnabled={true} />
  );
}

// Profile Screen
function ProfileScreen() {
  const [user, setUser] = useState<any>(null);
  useEffect(() => { IerahkwaAPI.me().then(setUser).catch(() => {}); }, []);
  return (
    <ScrollView style={s.container} contentContainerStyle={{ padding: 16 }}>
      <Text style={s.heading}>Profile</Text>
      {user ? (
        <>
          <View style={s.card}><Text style={s.cardLabel}>User ID</Text><Text style={s.cardValue}>{user.userId}</Text></View>
          <View style={s.card}><Text style={s.cardLabel}>Tenant</Text><Text style={s.cardValue}>{user.tenantId}</Text></View>
          <View style={s.card}><Text style={s.cardLabel}>Tier</Text><Text style={[s.cardValue, { color: COLORS.gold }]}>{user.tier}</Text></View>
          <View style={s.card}><Text style={s.cardLabel}>Roles</Text><Text style={s.cardValue}>{user.roles?.join(', ')}</Text></View>
        </>
      ) : (
        <ActivityIndicator color={COLORS.gold} size="large" />
      )}
    </ScrollView>
  );
}

// ===== NAVIGATION =====
const Tab = createBottomTabNavigator();
const Stack = createNativeStackNavigator();

function HomeTabs() {
  return (
    <Tab.Navigator screenOptions={{
      tabBarStyle: { backgroundColor: COLORS.bg2, borderTopColor: COLORS.brd },
      tabBarActiveTintColor: COLORS.gold,
      tabBarInactiveTintColor: COLORS.txt2,
      headerStyle: { backgroundColor: COLORS.bg2 },
      headerTintColor: COLORS.gold,
    }}>
      <Tab.Screen name="Home" component={HomeStack} options={{ headerShown: false, tabBarLabel: 'NEXUS' }} />
      <Tab.Screen name="Platforms" component={PlatformsScreen} options={{ tabBarLabel: 'Platforms' }} />
      <Tab.Screen name="Profile" component={ProfileScreen} options={{ tabBarLabel: 'Profile' }} />
    </Tab.Navigator>
  );
}

function HomeStack() {
  return (
    <Stack.Navigator screenOptions={{
      headerStyle: { backgroundColor: COLORS.bg2 },
      headerTintColor: COLORS.gold,
    }}>
      <Stack.Screen name="NEXUS" component={HomeScreen} options={{ title: 'Ierahkwa' }} />
      <Stack.Screen name="NexusDetail" component={NexusDetailScreen}
        options={({ route }: any) => ({ title: `NEXUS ${route.params.nexus.name}` })} />
    </Stack.Navigator>
  );
}

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        <Stack.Screen name="Login" component={LoginScreen} />
        <Stack.Screen name="Main" component={HomeTabs} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}

// ===== STYLES =====
const s = StyleSheet.create({
  container: { flex: 1, backgroundColor: COLORS.bg },
  loginBox: { flex: 1, justifyContent: 'center', padding: 32 },
  logoText: { fontSize: 36, fontWeight: '800', color: COLORS.gold, textAlign: 'center', letterSpacing: 4 },
  subtitle: { fontSize: 14, color: COLORS.txt2, textAlign: 'center', marginBottom: 40 },
  label: { fontSize: 11, color: COLORS.txt2, textTransform: 'uppercase', letterSpacing: 1, marginBottom: 6, marginTop: 16 },
  input: { backgroundColor: COLORS.bg2, borderWidth: 1, borderColor: COLORS.brd, borderRadius: 10, padding: 14, color: COLORS.txt, fontSize: 15 },
  btn: { backgroundColor: COLORS.gold, borderRadius: 10, padding: 16, alignItems: 'center', marginTop: 24 },
  btnText: { color: COLORS.bg, fontWeight: '700', fontSize: 16 },
  tierInfo: { fontSize: 11, color: COLORS.txt2, textAlign: 'center', marginTop: 24 },
  heading: { fontSize: 22, fontWeight: '700', color: COLORS.txt, marginBottom: 4 },
  subheading: { fontSize: 13, color: COLORS.txt2, marginBottom: 20 },
  nexusCard: { flexDirection: 'row', alignItems: 'center', backgroundColor: COLORS.bg2, borderWidth: 1, borderColor: COLORS.brd, borderLeftWidth: 4, borderRadius: 10, padding: 16, marginBottom: 10, gap: 14 },
  nexusName: { fontSize: 16, fontWeight: '700' },
  nexusDesc: { fontSize: 12, color: COLORS.txt2, marginTop: 2 },
  arrow: { fontSize: 24, color: COLORS.txt2 },
  card: { backgroundColor: COLORS.bg2, borderWidth: 1, borderColor: COLORS.brd, borderRadius: 10, padding: 16, marginBottom: 10 },
  cardLabel: { fontSize: 10, color: COLORS.txt2, textTransform: 'uppercase', letterSpacing: 1 },
  cardValue: { fontSize: 16, color: COLORS.txt, fontWeight: '600', marginTop: 4 },
});
