/** Ierahkwa Futurehead Shop - Config
 *  Mamey Node • Ierahkwa Sovereign Blockchain • IGT-MARKET
 */
import 'dotenv/config';

const dbUrl = process.env.DATABASE_URL || 'file:./data/ierahkwa_shop.db';
const isSqlite = dbUrl.startsWith('file:');

export default {
  port: parseInt(process.env.PORT || '3100', 10),
  env: process.env.NODE_ENV || 'development',
  db: {
    url: dbUrl,
    sqlite: isSqlite,
    sqlitePath: isSqlite ? dbUrl.replace(/^file:/, '').trim() : null,
  },
  jwt: { secret: process.env.JWT_SECRET || 'ierahkwa-shop-dev-secret' },
  stripe: {
    secret: process.env.STRIPE_SECRET_KEY || '',
    publishable: process.env.STRIPE_PUBLISHABLE_KEY || '',
    webhookSecret: process.env.STRIPE_WEBHOOK_SECRET || '',
  },
  site: {
    url: process.env.SITE_URL || 'http://localhost:3100',
    currency: process.env.CURRENCY_SYMBOL || '$',
    vat: parseFloat(process.env.VAT_PERCENT || '0'),
    meta: {
      description: process.env.META_DESCRIPTION || 'Ierahkwa Futurehead Shop',
      keywords: process.env.META_KEYWORDS || 'ierahkwa,shop,igt',
      googleAnalytics: process.env.GOOGLE_ANALYTICS_ID || '',
    },
    social: {
      facebook: process.env.SOCIAL_FACEBOOK || '',
      twitter: process.env.SOCIAL_TWITTER || '',
      instagram: process.env.SOCIAL_INSTAGRAM || '',
    },
    app: {
      ios: process.env.APP_IOS_LINK || '',
      android: process.env.APP_ANDROID_LINK || '',
    },
  },
  igt: {
    enabled: process.env.IGT_PAYMENT_ENABLED === 'true',
    apiUrl: process.env.IGT_API_URL || 'https://node.ierahkwa.gov',
    bdetUrl: process.env.BDET_SETTLEMENT_URL || 'https://pay.ierahkwa.gov',
  },
  ierahkwa: {
    node: 'Ierahkwa Futurehead Mamey Node',
    blockchain: 'Ierahkwa Sovereign Blockchain',
    bank: 'Ierahkwa Futurehead BDET Bank',
    government: 'Sovereign Government of Ierahkwa Ne Kanienke',
    tokenMarket: 'IGT-MARKET',
  },
};
