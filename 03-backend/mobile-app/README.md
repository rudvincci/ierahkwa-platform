# IERAHKWA Mobile App

## React Native Application
### Sovereign Government of Ierahkwa Ne Kanienke

---

## 📱 OVERVIEW

Aplicación móvil oficial del Gobierno Soberano de Ierahkwa. Disponible para iOS y Android.

## 🚀 FEATURES

- 🏛️ **Dashboard** - Vista general de la plataforma
- 💰 **Wallet** - Gestión de tokens y balances
- 💱 **Trade** - Swap y exchange de tokens
- 🗳️ **Governance** - Votación y propuestas
- 🏆 **Rewards** - Gamificación y logros
- 🌉 **Bridge** - Cross-chain transfers

## 🌐 MULTI-IDIOMA (i18n)

| Idioma | Código | Bandera |
|--------|--------|---------|
| English | `en` | 🇺🇸 |
| Español | `es` | 🇪🇸 |
| Kanien'kéha (Mohawk) | `moh` | 🪶 |
| Taíno | `tai` | 🌴 |

## 📁 ESTRUCTURA

```
mobile-app/
├── App.js                    # Entry point
├── package.json              # Dependencies
├── src/
│   ├── screens/
│   │   ├── DashboardScreen.js
│   │   ├── WalletScreen.js
│   │   ├── TradeScreen.js
│   │   ├── GovernanceScreen.js
│   │   ├── RewardsScreen.js
│   │   ├── BridgeScreen.js
│   │   ├── TokenDetailScreen.js
│   │   └── SettingsScreen.js
│   ├── components/
│   │   └── (shared components)
│   ├── services/
│   │   └── api.js            # API client
│   └── i18n/
│       └── index.js          # Translations
└── assets/
    └── (images, icons)
```

## 🔧 INSTALACIÓN

```bash
# Clonar repositorio
cd mobile-app

# Instalar dependencias
npm install

# iOS
cd ios && pod install && cd ..
npx react-native run-ios

# Android
npx react-native run-android
```

## 📡 API CONNECTION

La app se conecta al backend en:
- **Development:** `http://localhost:8545`
- **Production:** `https://api.ierahkwa.gov`

## 🎨 THEME

```javascript
colors: {
  primary: '#FFD700',      // Gold
  background: '#0a0e17',   // Dark
  card: '#1a1f2e',         // Card background
  success: '#00FF41',      // Green
  info: '#00FFFF',         // Cyan
  warning: '#FF6B35',      // Orange
  accent: '#9D4EDD',       // Purple
}
```

## 📱 SCREENS

### 1. Dashboard
- Estadísticas en tiempo real
- Quick actions
- Top tokens
- Network status

### 2. Wallet
- Balance total
- Lista de tokens
- Send/Receive
- Historial de transacciones

### 3. Trade
- Token swap
- Exchange rates
- Popular pairs
- Quick amounts

### 4. Governance
- Propuestas activas
- Sistema de votación
- Crear propuestas
- Resultados

### 5. Rewards
- Daily rewards
- Achievements
- Leaderboard
- Streak system

## 🔐 SECURITY

- Secure storage for keys
- Biometric authentication
- Encrypted communications
- No sensitive data in logs

## 📲 BUILD

```bash
# iOS Release
npx react-native run-ios --configuration Release

# Android APK
cd android && ./gradlew assembleRelease
```

---

**Version:** 1.0.0
**Platform:** iOS 14+ / Android 8+

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
