// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Real-time Trading Server - Node.js
// Version: 1.0.0
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

import express from 'express';
import { createServer } from 'http';
import { Server } from 'socket.io';
import cors from 'cors';
import helmet from 'helmet';
import compression from 'compression';
import { v4 as uuidv4 } from 'uuid';
import NodeCache from 'node-cache';

const app = express();
const server = createServer(app);
const corsOrigins = (process.env.CORS_ORIGINS || 'http://localhost:3000').split(',');
const io = new Server(server, {
    cors: {
        origin: corsOrigins,
        methods: ["GET", "POST"]
    }
});

// Cache for market data
const marketCache = new NodeCache({ stdTTL: 1 });
const signalCache = new NodeCache({ stdTTL: 300 });

// Middleware
app.use(helmet());
app.use(compression());
app.use(cors({ origin: corsOrigins, credentials: true }));
app.use(express.json());
app.use(express.static('public'));

// ============================================================================
// MARKET DATA SIMULATION
// ============================================================================

const FOREX_PAIRS = [
    { symbol: 'EURUSD', basePrice: 1.0850, pip: 0.0001, spread: 0.5 },
    { symbol: 'GBPUSD', basePrice: 1.2650, pip: 0.0001, spread: 0.8 },
    { symbol: 'USDJPY', basePrice: 149.50, pip: 0.01, spread: 0.5 },
    { symbol: 'USDCHF', basePrice: 0.8750, pip: 0.0001, spread: 0.8 },
    { symbol: 'AUDUSD', basePrice: 0.6550, pip: 0.0001, spread: 0.6 },
    { symbol: 'USDCAD', basePrice: 1.3550, pip: 0.0001, spread: 0.8 },
    { symbol: 'NZDUSD', basePrice: 0.6150, pip: 0.0001, spread: 1.0 },
    { symbol: 'EURJPY', basePrice: 162.20, pip: 0.01, spread: 1.0 },
    { symbol: 'GBPJPY', basePrice: 189.10, pip: 0.01, spread: 1.5 },
    { symbol: 'EURGBP', basePrice: 0.8580, pip: 0.0001, spread: 0.8 },
    { symbol: 'XAUUSD', basePrice: 2035.50, pip: 0.01, spread: 3.5 },
    { symbol: 'XAGUSD', basePrice: 23.15, pip: 0.01, spread: 2.0 },
    { symbol: 'BTCUSD', basePrice: 67500.00, pip: 0.01, spread: 50.0 },
    { symbol: 'ETHUSD', basePrice: 3250.00, pip: 0.01, spread: 5.0 }
];

const INDICES = [
    { symbol: 'US30', basePrice: 39250.00, pip: 1, spread: 2.5 },
    { symbol: 'US500', basePrice: 5150.00, pip: 0.1, spread: 0.5 },
    { symbol: 'US100', basePrice: 18150.00, pip: 1, spread: 1.5 },
    { symbol: 'DE40', basePrice: 17850.00, pip: 1, spread: 1.5 },
    { symbol: 'UK100', basePrice: 7680.00, pip: 0.1, spread: 1.0 }
];

const COMMODITIES = [
    { symbol: 'USOIL', basePrice: 78.50, pip: 0.01, spread: 0.03 },
    { symbol: 'UKOIL', basePrice: 83.20, pip: 0.01, spread: 0.03 },
    { symbol: 'NATGAS', basePrice: 2.15, pip: 0.001, spread: 0.005 }
];

const ALL_INSTRUMENTS = [...FOREX_PAIRS, ...INDICES, ...COMMODITIES];

// Store for prices
const currentPrices = new Map();

// Initialize prices
ALL_INSTRUMENTS.forEach(inst => {
    currentPrices.set(inst.symbol, {
        symbol: inst.symbol,
        bid: inst.basePrice,
        ask: inst.basePrice + (inst.spread * inst.pip),
        high: inst.basePrice * 1.002,
        low: inst.basePrice * 0.998,
        change: 0,
        changePercent: 0,
        volume: Math.floor(Math.random() * 1000000),
        timestamp: Date.now()
    });
});

// Price update simulation
function updatePrices() {
    ALL_INSTRUMENTS.forEach(inst => {
        const current = currentPrices.get(inst.symbol);
        const volatility = inst.pip * (Math.random() * 20 - 10);
        const newBid = current.bid + volatility;
        const newAsk = newBid + (inst.spread * inst.pip);
        
        const change = newBid - inst.basePrice;
        const changePercent = (change / inst.basePrice) * 100;
        
        currentPrices.set(inst.symbol, {
            symbol: inst.symbol,
            bid: parseFloat(newBid.toFixed(inst.pip < 0.01 ? 5 : 2)),
            ask: parseFloat(newAsk.toFixed(inst.pip < 0.01 ? 5 : 2)),
            high: Math.max(current.high, newBid),
            low: Math.min(current.low, newBid),
            change: parseFloat(change.toFixed(inst.pip < 0.01 ? 5 : 2)),
            changePercent: parseFloat(changePercent.toFixed(2)),
            volume: current.volume + Math.floor(Math.random() * 1000),
            timestamp: Date.now()
        });
    });
}

// Update prices every 100ms
setInterval(updatePrices, 100);

// ============================================================================
// INVESTMENT PLANS DATA
// ============================================================================

const INVESTMENT_PLANS = [
    {
        id: 'plan-starter',
        name: 'Starter Plan',
        description: 'Perfect for beginners starting their investment journey',
        type: 'Standard',
        category: 'Forex',
        minAmount: 100,
        maxAmount: 5000,
        minROI: 5,
        maxROI: 12,
        currency: 'USD',
        riskLevel: 'Low',
        isTrending: false,
        isFeatured: true,
        imageUrl: '/images/plan-starter.png',
        durations: [
            { id: 'd1', name: '24 Hours', type: 'Hourly', value: 24, roiBonus: 0 },
            { id: 'd2', name: '7 Days', type: 'Daily', value: 7, roiBonus: 1 },
            { id: 'd3', name: '30 Days', type: 'Daily', value: 30, roiBonus: 2 }
        ]
    },
    {
        id: 'plan-growth',
        name: 'Growth Plan',
        description: 'Balanced returns with moderate risk exposure',
        type: 'Standard',
        category: 'Mixed',
        minAmount: 1000,
        maxAmount: 25000,
        minROI: 10,
        maxROI: 20,
        currency: 'USD',
        riskLevel: 'Medium',
        isTrending: true,
        isFeatured: true,
        imageUrl: '/images/plan-growth.png',
        durations: [
            { id: 'd2', name: '7 Days', type: 'Daily', value: 7, roiBonus: 0 },
            { id: 'd3', name: '30 Days', type: 'Daily', value: 30, roiBonus: 3 },
            { id: 'd4', name: '90 Days', type: 'Daily', value: 90, roiBonus: 5 }
        ]
    },
    {
        id: 'plan-premium',
        name: 'Premium Plan',
        description: 'Higher returns for experienced investors',
        type: 'Premium',
        category: 'Forex',
        minAmount: 5000,
        maxAmount: 100000,
        minROI: 15,
        maxROI: 30,
        currency: 'USD',
        riskLevel: 'High',
        isTrending: true,
        isFeatured: false,
        imageUrl: '/images/plan-premium.png',
        durations: [
            { id: 'd3', name: '30 Days', type: 'Daily', value: 30, roiBonus: 0 },
            { id: 'd4', name: '90 Days', type: 'Daily', value: 90, roiBonus: 5 },
            { id: 'd5', name: '180 Days', type: 'Daily', value: 180, roiBonus: 8 }
        ]
    },
    {
        id: 'plan-vip',
        name: 'VIP Elite Plan',
        description: 'Exclusive high-yield opportunities for VIP members',
        type: 'VIP',
        category: 'Mixed',
        minAmount: 25000,
        maxAmount: 500000,
        minROI: 20,
        maxROI: 45,
        currency: 'USD',
        riskLevel: 'VeryHigh',
        isTrending: false,
        isFeatured: true,
        imageUrl: '/images/plan-vip.png',
        durations: [
            { id: 'd4', name: '90 Days', type: 'Daily', value: 90, roiBonus: 0 },
            { id: 'd5', name: '180 Days', type: 'Daily', value: 180, roiBonus: 5 },
            { id: 'd6', name: '365 Days', type: 'Yearly', value: 1, roiBonus: 10 }
        ]
    },
    {
        id: 'plan-institutional',
        name: 'Institutional Plan',
        description: 'Tailored solutions for institutional investors',
        type: 'Institutional',
        category: 'Mixed',
        minAmount: 100000,
        maxAmount: 10000000,
        minROI: 25,
        maxROI: 60,
        currency: 'USD',
        riskLevel: 'Custom',
        isTrending: false,
        isFeatured: false,
        imageUrl: '/images/plan-institutional.png',
        durations: [
            { id: 'd5', name: '180 Days', type: 'Daily', value: 180, roiBonus: 0 },
            { id: 'd6', name: '365 Days', type: 'Yearly', value: 1, roiBonus: 10 }
        ]
    }
];

// ============================================================================
// SIGNAL PROVIDERS DATA
// ============================================================================

const SIGNAL_PROVIDERS = [
    {
        id: 'sp-alpha',
        name: 'Alpha Traders Pro',
        description: 'Professional forex signals with 85%+ win rate',
        avatarUrl: '/images/provider-alpha.png',
        isVerified: true,
        totalProfit: 45230.50,
        winRate: 85.5,
        totalTrades: 1250,
        averageROI: 12.5,
        maxDrawdown: 8.2,
        monthlyFee: 99,
        performanceFee: 20,
        tradingStyle: 'Swing',
        riskLevel: 'Medium',
        rating: 4.8,
        reviewCount: 342,
        currentSubscribers: 856
    },
    {
        id: 'sp-quantum',
        name: 'Quantum Signals AI',
        description: 'AI-powered algorithmic trading signals',
        avatarUrl: '/images/provider-quantum.png',
        isVerified: true,
        totalProfit: 128500.75,
        winRate: 78.3,
        totalTrades: 3420,
        averageROI: 18.2,
        maxDrawdown: 12.5,
        monthlyFee: 199,
        performanceFee: 25,
        tradingStyle: 'Algorithmic',
        riskLevel: 'High',
        rating: 4.6,
        reviewCount: 512,
        currentSubscribers: 1245
    },
    {
        id: 'sp-scalper',
        name: 'Scalper Master',
        description: 'High-frequency scalping signals for quick profits',
        avatarUrl: '/images/provider-scalper.png',
        isVerified: true,
        totalProfit: 32150.25,
        winRate: 72.8,
        totalTrades: 8500,
        averageROI: 8.5,
        maxDrawdown: 15.0,
        monthlyFee: 149,
        performanceFee: 15,
        tradingStyle: 'Scalping',
        riskLevel: 'VeryHigh',
        rating: 4.3,
        reviewCount: 287,
        currentSubscribers: 623
    }
];

// Active signals storage
const activeSignals = new Map();

// Generate random signal
function generateSignal(providerId) {
    const provider = SIGNAL_PROVIDERS.find(p => p.id === providerId) || SIGNAL_PROVIDERS[0];
    const instrument = FOREX_PAIRS[Math.floor(Math.random() * FOREX_PAIRS.length)];
    const price = currentPrices.get(instrument.symbol);
    const isBuy = Math.random() > 0.5;
    
    const pipValue = instrument.pip;
    const sl = isBuy ? price.bid - (30 * pipValue) : price.ask + (30 * pipValue);
    const tp1 = isBuy ? price.bid + (20 * pipValue) : price.ask - (20 * pipValue);
    const tp2 = isBuy ? price.bid + (40 * pipValue) : price.ask - (40 * pipValue);
    const tp3 = isBuy ? price.bid + (60 * pipValue) : price.ask - (60 * pipValue);
    
    return {
        id: uuidv4(),
        providerId: provider.id,
        providerName: provider.name,
        symbol: instrument.symbol,
        type: 'Manual',
        action: isBuy ? 'Buy' : 'Sell',
        entryPrice: isBuy ? price.ask : price.bid,
        stopLoss: parseFloat(sl.toFixed(5)),
        takeProfit1: parseFloat(tp1.toFixed(5)),
        takeProfit2: parseFloat(tp2.toFixed(5)),
        takeProfit3: parseFloat(tp3.toFixed(5)),
        riskRewardRatio: 2.0,
        lotSize: 0.1,
        status: 'Active',
        analysis: `${isBuy ? 'Bullish' : 'Bearish'} momentum detected on ${instrument.symbol}. Price action suggests a ${isBuy ? 'long' : 'short'} opportunity.`,
        createdAt: new Date().toISOString(),
        expiresAt: new Date(Date.now() + 4 * 60 * 60 * 1000).toISOString()
    };
}

// ============================================================================
// SOCKET.IO EVENTS
// ============================================================================

io.on('connection', (socket) => {
    console.log(`Client connected: ${socket.id}`);
    
    // Send initial market data
    socket.emit('market:init', Array.from(currentPrices.values()));
    
    // Subscribe to market updates
    socket.on('market:subscribe', (symbols) => {
        socket.join('market-updates');
        console.log(`Client ${socket.id} subscribed to market updates`);
    });
    
    // Subscribe to signals
    socket.on('signals:subscribe', (providerId) => {
        socket.join(`signals-${providerId}`);
        console.log(`Client ${socket.id} subscribed to signals from ${providerId}`);
    });
    
    // Create new signal (for providers)
    socket.on('signal:create', (data) => {
        const signal = generateSignal(data.providerId);
        activeSignals.set(signal.id, signal);
        io.to(`signals-${data.providerId}`).emit('signal:new', signal);
        console.log(`New signal created: ${signal.symbol} ${signal.action}`);
    });
    
    // Close signal
    socket.on('signal:close', (data) => {
        const signal = activeSignals.get(data.signalId);
        if (signal) {
            signal.status = 'Closed';
            signal.result = data.result;
            signal.exitPrice = data.exitPrice;
            signal.closedAt = new Date().toISOString();
            io.to(`signals-${signal.providerId}`).emit('signal:closed', signal);
        }
    });
    
    // Investment operations
    socket.on('investment:create', (data) => {
        const investment = {
            id: uuidv4(),
            ...data,
            status: 'Active',
            createdAt: new Date().toISOString(),
            progress: 0
        };
        socket.emit('investment:created', investment);
        console.log(`Investment created: ${investment.id} - ${data.amount} ${data.currency}`);
    });
    
    socket.on('disconnect', () => {
        console.log(`Client disconnected: ${socket.id}`);
    });
});

// Broadcast market updates every 100ms
setInterval(() => {
    const updates = Array.from(currentPrices.values());
    io.to('market-updates').emit('market:update', updates);
}, 100);

// ============================================================================
// REST API ENDPOINTS
// ============================================================================

// Health check
app.get('/health', (req, res) => {
    res.json({ 
        status: 'healthy', 
        server: 'Ierahkwa Forex Trading Server',
        version: '1.0.0',
        timestamp: new Date().toISOString()
    });
});

// Get all instruments
app.get('/api/instruments', (req, res) => {
    res.json({
        forex: FOREX_PAIRS.map(p => ({ ...p, ...currentPrices.get(p.symbol) })),
        indices: INDICES.map(p => ({ ...p, ...currentPrices.get(p.symbol) })),
        commodities: COMMODITIES.map(p => ({ ...p, ...currentPrices.get(p.symbol) }))
    });
});

// Get market data for symbol
app.get('/api/market/:symbol', (req, res) => {
    const data = currentPrices.get(req.params.symbol.toUpperCase());
    if (data) {
        res.json(data);
    } else {
        res.status(404).json({ error: 'Symbol not found' });
    }
});

// Get all investment plans
app.get('/api/plans', (req, res) => {
    res.json(INVESTMENT_PLANS);
});

// Get trending plans
app.get('/api/plans/trending', (req, res) => {
    res.json(INVESTMENT_PLANS.filter(p => p.isTrending));
});

// Get plan by ID
app.get('/api/plans/:id', (req, res) => {
    const plan = INVESTMENT_PLANS.find(p => p.id === req.params.id);
    if (plan) {
        res.json(plan);
    } else {
        res.status(404).json({ error: 'Plan not found' });
    }
});

// Get all signal providers
app.get('/api/signals/providers', (req, res) => {
    res.json(SIGNAL_PROVIDERS);
});

// Get provider by ID
app.get('/api/signals/providers/:id', (req, res) => {
    const provider = SIGNAL_PROVIDERS.find(p => p.id === req.params.id);
    if (provider) {
        res.json(provider);
    } else {
        res.status(404).json({ error: 'Provider not found' });
    }
});

// Get active signals for provider
app.get('/api/signals/providers/:id/signals', (req, res) => {
    const signals = Array.from(activeSignals.values())
        .filter(s => s.providerId === req.params.id && s.status === 'Active');
    res.json(signals);
});

// Calculate investment
app.post('/api/calculate', (req, res) => {
    const { planId, durationId, amount } = req.body;
    
    const plan = INVESTMENT_PLANS.find(p => p.id === planId);
    if (!plan) {
        return res.status(400).json({ error: 'Invalid plan' });
    }
    
    const duration = plan.durations.find(d => d.id === durationId);
    if (!duration) {
        return res.status(400).json({ error: 'Invalid duration' });
    }
    
    if (amount < plan.minAmount || amount > plan.maxAmount) {
        return res.status(400).json({ 
            error: `Amount must be between ${plan.minAmount} and ${plan.maxAmount}` 
        });
    }
    
    const baseROI = (plan.minROI + plan.maxROI) / 2;
    const totalROI = baseROI + duration.roiBonus;
    const profit = amount * (totalROI / 100);
    
    res.json({
        planName: plan.name,
        durationName: duration.name,
        amount,
        currency: plan.currency,
        roi: totalROI,
        expectedProfit: parseFloat(profit.toFixed(2)),
        totalReturn: parseFloat((amount + profit).toFixed(2)),
        riskLevel: plan.riskLevel
    });
});

// Analytics endpoint
app.get('/api/analytics/platform', (req, res) => {
    res.json({
        totalInvested: 15750000,
        totalProfit: 2890000,
        totalUsers: 12500,
        activeUsers: 8200,
        totalInvestments: 45600,
        activeInvestments: 12300,
        totalDeposits: 28500000,
        totalWithdrawals: 12750000,
        investmentsByPlan: {
            'Starter Plan': 3250000,
            'Growth Plan': 5500000,
            'Premium Plan': 4200000,
            'VIP Elite Plan': 2100000,
            'Institutional Plan': 700000
        }
    });
});

// ============================================================================
// START SERVER
// ============================================================================

const PORT = process.env.PORT || 3500;

server.listen(PORT, () => {
    console.log('═══════════════════════════════════════════════════════════════');
    console.log('  IERAHKWA FUTUREHEAD - FOREX TRADING SERVER');
    console.log('  © 2026 Ierahkwa Ne Kanienke Sovereign Government');
    console.log('═══════════════════════════════════════════════════════════════');
    console.log(`  Server running on port ${PORT}`);
    console.log(`  WebSocket enabled for real-time trading`);
    console.log(`  Market data: ${ALL_INSTRUMENTS.length} instruments`);
    console.log(`  Investment plans: ${INVESTMENT_PLANS.length} available`);
    console.log(`  Signal providers: ${SIGNAL_PROVIDERS.length} active`);
    console.log('═══════════════════════════════════════════════════════════════');
});
