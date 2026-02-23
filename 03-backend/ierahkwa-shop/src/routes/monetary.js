/** 
 * ╔═══════════════════════════════════════════════════════════════════════════╗
 * ║      IERAHKWA SOVEREIGN MONETARY SYSTEM - M0 TO M4                       ║
 * ║                                                                           ║
 * ║  Asset-Backed Currency Classifications:                                   ║
 * ║  • M0 - Monetary Base: Cash, Fiat Currency, Reserves                     ║
 * ║  • M1 - Precious Assets: Diamonds, Rubies, Emeralds, Precious Stones     ║
 * ║  • M2 - Real Assets: Houses, Apartments, Properties, Land                ║
 * ║  • M3 - Large Deposits + Financial Instruments                           ║
 * ║  • M4 - Commercial Paper + Bonds + Securities                            ║
 * ║                                                                           ║
 * ║  Humanitarian Fund Allocation on Conversion                              ║
 * ║  Ierahkwa Sovereign Blockchain (ISB) • IGT Tokens                        ║
 * ╚═══════════════════════════════════════════════════════════════════════════╝
 */
import db from '../db.js';

// ============================================
// MONETARY CLASSIFICATIONS (M0-M4)
// ============================================

const MONETARY_CLASSIFICATIONS = {
  M0: {
    code: 'M0',
    name: 'Monetary Base',
    description: 'Currency + Reserves (Cash, Fiat, Central Bank Reserves)',
    color: '#22d3ee', // cyan
    assets: [
      { type: 'CASH', name: 'Physical Cash', unit: 'units' },
      { type: 'FIAT', name: 'Fiat Currency', unit: 'units' },
      { type: 'RESERVES', name: 'Central Bank Reserves', unit: 'units' },
      { type: 'COINS', name: 'Coins & Bullion Coins', unit: 'oz' }
    ],
    liquidity: 'Highest',
    conversion_rate: 1.0,
    humanitarian_percent: 0 // Base currency, no conversion needed
  },
  M1: {
    code: 'M1',
    name: 'M0 + Precious Assets',
    description: 'Diamonds, Rubies, Emeralds, Sapphires, Precious Stones',
    color: '#f59e0b', // amber/gold
    assets: [
      { type: 'DIAMOND', name: 'Diamonds', unit: 'carats', price_per_unit: 5000 },
      { type: 'RUBY', name: 'Rubies', unit: 'carats', price_per_unit: 3000 },
      { type: 'EMERALD', name: 'Emeralds', unit: 'carats', price_per_unit: 4000 },
      { type: 'SAPPHIRE', name: 'Sapphires', unit: 'carats', price_per_unit: 2500 },
      { type: 'GOLD', name: 'Gold Bullion', unit: 'oz', price_per_unit: 2000 },
      { type: 'SILVER', name: 'Silver Bullion', unit: 'oz', price_per_unit: 25 },
      { type: 'PLATINUM', name: 'Platinum', unit: 'oz', price_per_unit: 1000 },
      { type: 'PALLADIUM', name: 'Palladium', unit: 'oz', price_per_unit: 1200 }
    ],
    liquidity: 'High',
    conversion_rate: 0.95,
    humanitarian_percent: 10 // 10% goes to humanitarian fund on conversion
  },
  M2: {
    code: 'M2',
    name: 'M1 + Real Assets',
    description: 'Houses, Apartments, Properties, Land, Real Estate',
    color: '#10b981', // green
    assets: [
      { type: 'HOUSE', name: 'Houses', unit: 'property', avg_value: 350000 },
      { type: 'APARTMENT', name: 'Apartments', unit: 'unit', avg_value: 200000 },
      { type: 'LAND', name: 'Land Parcels', unit: 'acres', price_per_unit: 10000 },
      { type: 'COMMERCIAL', name: 'Commercial Property', unit: 'sqft', price_per_unit: 200 },
      { type: 'INDUSTRIAL', name: 'Industrial Property', unit: 'sqft', price_per_unit: 100 },
      { type: 'AGRICULTURAL', name: 'Agricultural Land', unit: 'acres', price_per_unit: 5000 }
    ],
    liquidity: 'Medium',
    conversion_rate: 0.90,
    humanitarian_percent: 15 // 15% goes to humanitarian fund
  },
  M3: {
    code: 'M3',
    name: 'M2 + Large Deposits',
    description: 'Large Time Deposits, Institutional Funds, Repos',
    color: '#8b5cf6', // purple
    assets: [
      { type: 'TIME_DEPOSIT', name: 'Large Time Deposits', unit: 'USD', min_amount: 100000 },
      { type: 'INST_FUND', name: 'Institutional Funds', unit: 'USD', min_amount: 1000000 },
      { type: 'REPO', name: 'Repurchase Agreements', unit: 'USD', min_amount: 500000 },
      { type: 'MONEY_MARKET', name: 'Money Market Funds', unit: 'USD', min_amount: 50000 }
    ],
    liquidity: 'Medium-Low',
    conversion_rate: 0.85,
    humanitarian_percent: 20 // 20% goes to humanitarian fund
  },
  M4: {
    code: 'M4',
    name: 'M3 + Commercial Paper + Bonds',
    description: 'Commercial Paper, Government Bonds, Corporate Bonds, Securities',
    color: '#ec4899', // pink
    assets: [
      { type: 'COMMERCIAL_PAPER', name: 'Commercial Paper', unit: 'USD', maturity: '270 days' },
      { type: 'GOV_BOND', name: 'Government Bonds', unit: 'USD', maturity: '1-30 years' },
      { type: 'CORP_BOND', name: 'Corporate Bonds', unit: 'USD', maturity: '1-30 years' },
      { type: 'TREASURY', name: 'Treasury Bills', unit: 'USD', maturity: '1 year' },
      { type: 'MUNICIPAL', name: 'Municipal Bonds', unit: 'USD', maturity: '1-30 years' },
      { type: 'SOVEREIGN', name: 'Sovereign Bonds', unit: 'USD', maturity: '1-30 years' }
    ],
    liquidity: 'Low',
    conversion_rate: 0.80,
    humanitarian_percent: 25 // 25% goes to humanitarian fund
  }
};

// Humanitarian Fund Configuration
const HUMANITARIAN_CONFIG = {
  fund_name: 'Ierahkwa Global Humanitarian Fund',
  purposes: [
    { id: 'MEDICAL', name: 'Medical Aid & Healthcare', allocation: 25 },
    { id: 'EDUCATION', name: 'Education & Schools', allocation: 20 },
    { id: 'FOOD', name: 'Food Security & Agriculture', allocation: 20 },
    { id: 'HOUSING', name: 'Housing & Shelter', allocation: 15 },
    { id: 'WATER', name: 'Clean Water & Sanitation', allocation: 10 },
    { id: 'DISASTER', name: 'Disaster Relief', allocation: 10 }
  ],
  minimum_conversion: 1000, // Minimum amount to trigger humanitarian allocation
  max_humanitarian_percent: 30 // Maximum percentage for humanitarian
};

// ============================================
// INITIALIZE MONETARY TABLES
// ============================================

function initMonetaryTables() {
  const data = db.get();
  
  // Monetary Classifications Reference
  if (!data.monetary_classifications) {
    data.monetary_classifications = MONETARY_CLASSIFICATIONS;
  }
  
  // Humanitarian Fund
  if (!data.humanitarian_fund) {
    data.humanitarian_fund = {
      config: HUMANITARIAN_CONFIG,
      total_collected: 0,
      total_distributed: 0,
      balance: 0,
      allocations: HUMANITARIAN_CONFIG.purposes.map(p => ({
        ...p,
        collected: 0,
        distributed: 0,
        balance: 0
      })),
      transactions: [],
      created_at: new Date().toISOString()
    };
  }
  
  // Asset Holdings (for each entity)
  if (!data.asset_holdings) {
    data.asset_holdings = [];
    data._counters.asset_holdings = 0;
  }
  
  // Asset Conversions
  if (!data.asset_conversions) {
    data.asset_conversions = [];
    data._counters.asset_conversions = 0;
  }
  
  // M-Class Balances for entities
  if (!data.mclass_balances) {
    data.mclass_balances = {};
  }
  
  db.save();
}

export default async function monetaryRoutes(fastify) {
  initMonetaryTables();

  // ==================== CLASSIFICATIONS ====================
  
  fastify.get('/api/monetary/classifications', async () => {
    return {
      classifications: MONETARY_CLASSIFICATIONS,
      humanitarian_config: HUMANITARIAN_CONFIG
    };
  });

  fastify.get('/api/monetary/classifications/:code', async (req) => {
    const code = req.params.code.toUpperCase();
    const classification = MONETARY_CLASSIFICATIONS[code];
    if (!classification) return { error: 'Classification not found' };
    return classification;
  });

  // ==================== ASSET HOLDINGS ====================
  
  fastify.get('/api/monetary/holdings', async (req) => {
    const data = db.get();
    let holdings = data.asset_holdings || [];
    
    if (req.query.entity_type) {
      holdings = holdings.filter(h => h.entity_type === req.query.entity_type);
    }
    if (req.query.entity_id) {
      holdings = holdings.filter(h => h.entity_id === req.query.entity_id);
    }
    if (req.query.mclass) {
      holdings = holdings.filter(h => h.mclass === req.query.mclass.toUpperCase());
    }
    
    return holdings;
  });

  fastify.post('/api/monetary/holdings', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, entity_name, mclass, asset_type, quantity, value_usd, certificate_number, notes } = req.body;
    
    const classification = MONETARY_CLASSIFICATIONS[mclass?.toUpperCase()];
    if (!classification) return { error: 'Invalid monetary classification' };
    
    const asset = classification.assets.find(a => a.type === asset_type);
    if (!asset) return { error: 'Invalid asset type for this classification' };
    
    const holdingId = db.nextId('asset_holdings');
    const holding = {
      id: holdingId,
      entity_type,
      entity_id,
      entity_name,
      mclass: mclass.toUpperCase(),
      mclass_name: classification.name,
      asset_type,
      asset_name: asset.name,
      quantity: parseFloat(quantity),
      unit: asset.unit,
      value_usd: parseFloat(value_usd) || (parseFloat(quantity) * (asset.price_per_unit || asset.avg_value || 1)),
      certificate_number: certificate_number || `CERT-${mclass}-${holdingId}-${Date.now()}`,
      status: 'active',
      notes: notes || '',
      created_at: db.now(),
      updated_at: db.now()
    };
    
    data.asset_holdings.push(holding);
    
    // Update M-Class balance for entity
    const balanceKey = `${entity_type}-${entity_id}`;
    if (!data.mclass_balances[balanceKey]) {
      data.mclass_balances[balanceKey] = { M0: 0, M1: 0, M2: 0, M3: 0, M4: 0 };
    }
    data.mclass_balances[balanceKey][mclass.toUpperCase()] += holding.value_usd;
    
    db.save();
    return { ok: true, holding };
  });

  fastify.put('/api/monetary/holdings/:id', async (req) => {
    const data = db.get();
    const id = parseInt(req.params.id);
    const holding = (data.asset_holdings || []).find(h => h.id === id);
    
    if (!holding) return { error: 'Holding not found' };
    
    const { quantity, value_usd, status, notes } = req.body;
    
    if (quantity !== undefined) holding.quantity = parseFloat(quantity);
    if (value_usd !== undefined) holding.value_usd = parseFloat(value_usd);
    if (status !== undefined) holding.status = status;
    if (notes !== undefined) holding.notes = notes;
    holding.updated_at = db.now();
    
    db.save();
    return { ok: true, holding };
  });

  // ==================== ASSET CONVERSION (M1-M4 → M0) ====================
  
  fastify.post('/api/monetary/convert', async (req) => {
    const data = db.get();
    const { entity_type, entity_id, entity_name, from_mclass, holding_ids, convert_to_igt } = req.body;
    
    const fromClass = MONETARY_CLASSIFICATIONS[from_mclass?.toUpperCase()];
    if (!fromClass) return { error: 'Invalid source classification' };
    if (from_mclass.toUpperCase() === 'M0') return { error: 'M0 is already base currency' };
    
    // Get holdings to convert
    const holdingsToConvert = (data.asset_holdings || []).filter(h => 
      holding_ids.includes(h.id) && 
      h.entity_type === entity_type && 
      h.entity_id === entity_id &&
      h.status === 'active'
    );
    
    if (holdingsToConvert.length === 0) return { error: 'No valid holdings found to convert' };
    
    // Calculate total value
    const totalValue = holdingsToConvert.reduce((sum, h) => sum + h.value_usd, 0);
    
    // Apply conversion rate
    const convertedValue = totalValue * fromClass.conversion_rate;
    
    // Calculate humanitarian allocation
    const humanitarianPercent = fromClass.humanitarian_percent;
    const humanitarianAmount = totalValue >= HUMANITARIAN_CONFIG.minimum_conversion 
      ? convertedValue * (humanitarianPercent / 100)
      : 0;
    
    const netAmount = convertedValue - humanitarianAmount;
    
    // Mark holdings as converted
    holdingsToConvert.forEach(h => {
      h.status = 'converted';
      h.converted_at = db.now();
      h.conversion_id = db.nextId('asset_conversions');
    });
    
    // Record conversion
    const conversionId = data._counters.asset_conversions;
    const conversion = {
      id: conversionId,
      entity_type,
      entity_id,
      entity_name,
      from_mclass: from_mclass.toUpperCase(),
      to_mclass: 'M0',
      holdings_converted: holdingsToConvert.map(h => h.id),
      original_value: totalValue,
      conversion_rate: fromClass.conversion_rate,
      converted_value: convertedValue,
      humanitarian_percent: humanitarianPercent,
      humanitarian_amount: humanitarianAmount,
      net_amount: netAmount,
      convert_to_igt: convert_to_igt || false,
      igt_amount: convert_to_igt ? netAmount : 0, // 1:1 IGT to USD
      status: 'completed',
      created_at: db.now()
    };
    data.asset_conversions.push(conversion);
    
    // Update humanitarian fund
    if (humanitarianAmount > 0) {
      data.humanitarian_fund.total_collected += humanitarianAmount;
      data.humanitarian_fund.balance += humanitarianAmount;
      
      // Distribute to purposes based on allocation percentages
      HUMANITARIAN_CONFIG.purposes.forEach(purpose => {
        const allocation = data.humanitarian_fund.allocations.find(a => a.id === purpose.id);
        if (allocation) {
          const amount = humanitarianAmount * (purpose.allocation / 100);
          allocation.collected += amount;
          allocation.balance += amount;
        }
      });
      
      // Record humanitarian transaction
      data.humanitarian_fund.transactions.push({
        id: data.humanitarian_fund.transactions.length + 1,
        type: 'collection',
        source_conversion_id: conversionId,
        source_entity: entity_name,
        source_mclass: from_mclass.toUpperCase(),
        amount: humanitarianAmount,
        created_at: db.now()
      });
    }
    
    // Credit entity's bank account if converting to IGT
    if (convert_to_igt) {
      // Find entity and credit their account
      let entity;
      if (entity_type === 'central_bank') {
        entity = (data.central_banks || []).find(e => e.id === entity_id);
      } else if (entity_type === 'futurehead') {
        entity = (data.futurehead_entities || []).find(e => e.id === entity_id);
      } else if (entity_type === 'government') {
        entity = (data.government_accounts || []).find(e => e.id === entity_id);
      } else if (entity_type === 'department') {
        entity = (data.department_accounts || []).find(e => e.id === parseInt(entity_id));
      }
      
      if (entity && entity.bank_account) {
        entity.bank_account.igt_balance = (entity.bank_account.igt_balance || 0) + netAmount;
      }
    }
    
    // Update M-Class balances
    const balanceKey = `${entity_type}-${entity_id}`;
    if (data.mclass_balances[balanceKey]) {
      data.mclass_balances[balanceKey][from_mclass.toUpperCase()] -= totalValue;
      data.mclass_balances[balanceKey]['M0'] += netAmount;
    }
    
    db.save();
    
    return {
      ok: true,
      conversion: {
        id: conversionId,
        original_value: totalValue,
        conversion_rate: fromClass.conversion_rate,
        converted_value: convertedValue,
        humanitarian_contribution: humanitarianAmount,
        humanitarian_percent: humanitarianPercent,
        net_received: netAmount,
        igt_credited: convert_to_igt ? netAmount : 0
      }
    };
  });

  fastify.get('/api/monetary/conversions', async (req) => {
    const data = db.get();
    let conversions = data.asset_conversions || [];
    
    if (req.query.entity_type) {
      conversions = conversions.filter(c => c.entity_type === req.query.entity_type);
    }
    if (req.query.entity_id) {
      conversions = conversions.filter(c => c.entity_id === req.query.entity_id);
    }
    
    return conversions.slice(-100).reverse();
  });

  // ==================== HUMANITARIAN FUND ====================
  
  fastify.get('/api/monetary/humanitarian', async () => {
    const data = db.get();
    return data.humanitarian_fund || {};
  });

  fastify.get('/api/monetary/humanitarian/transactions', async (req) => {
    const data = db.get();
    let transactions = data.humanitarian_fund?.transactions || [];
    
    if (req.query.type) {
      transactions = transactions.filter(t => t.type === req.query.type);
    }
    
    return transactions.slice(-100).reverse();
  });

  fastify.post('/api/monetary/humanitarian/distribute', async (req) => {
    const data = db.get();
    const { purpose_id, amount, recipient, description } = req.body;
    
    const allocation = data.humanitarian_fund.allocations.find(a => a.id === purpose_id);
    if (!allocation) return { error: 'Invalid purpose' };
    if (allocation.balance < amount) return { error: 'Insufficient funds in this allocation' };
    
    allocation.distributed += parseFloat(amount);
    allocation.balance -= parseFloat(amount);
    data.humanitarian_fund.total_distributed += parseFloat(amount);
    data.humanitarian_fund.balance -= parseFloat(amount);
    
    data.humanitarian_fund.transactions.push({
      id: data.humanitarian_fund.transactions.length + 1,
      type: 'distribution',
      purpose_id,
      purpose_name: allocation.name,
      amount: parseFloat(amount),
      recipient,
      description,
      created_at: db.now()
    });
    
    db.save();
    return { ok: true, new_balance: allocation.balance };
  });

  // ==================== ENTITY BALANCES ====================
  
  fastify.get('/api/monetary/balances/:entity_type/:entity_id', async (req) => {
    const data = db.get();
    const { entity_type, entity_id } = req.params;
    const balanceKey = `${entity_type}-${entity_id}`;
    
    const balances = data.mclass_balances[balanceKey] || { M0: 0, M1: 0, M2: 0, M3: 0, M4: 0 };
    
    // Get holdings breakdown
    const holdings = (data.asset_holdings || []).filter(h => 
      h.entity_type === entity_type && 
      h.entity_id === entity_id &&
      h.status === 'active'
    );
    
    const holdingsByClass = {
      M0: holdings.filter(h => h.mclass === 'M0'),
      M1: holdings.filter(h => h.mclass === 'M1'),
      M2: holdings.filter(h => h.mclass === 'M2'),
      M3: holdings.filter(h => h.mclass === 'M3'),
      M4: holdings.filter(h => h.mclass === 'M4')
    };
    
    const totalValue = Object.values(balances).reduce((sum, v) => sum + v, 0);
    
    return {
      entity_type,
      entity_id,
      balances,
      total_value: totalValue,
      holdings_count: holdings.length,
      holdings_by_class: holdingsByClass
    };
  });

  // ==================== DASHBOARD ====================
  
  fastify.get('/api/monetary/dashboard', async () => {
    const data = db.get();
    
    const holdings = data.asset_holdings || [];
    const activeHoldings = holdings.filter(h => h.status === 'active');
    const conversions = data.asset_conversions || [];
    const humanitarian = data.humanitarian_fund || {};
    
    // Calculate totals by M-Class
    const totalsByClass = { M0: 0, M1: 0, M2: 0, M3: 0, M4: 0 };
    activeHoldings.forEach(h => {
      totalsByClass[h.mclass] = (totalsByClass[h.mclass] || 0) + h.value_usd;
    });
    
    // Recent conversions
    const recentConversions = conversions.slice(-10).reverse();
    
    return {
      stats: {
        total_holdings: activeHoldings.length,
        total_value: Object.values(totalsByClass).reduce((s, v) => s + v, 0),
        total_conversions: conversions.length,
        total_converted_value: conversions.reduce((s, c) => s + c.original_value, 0),
        humanitarian_collected: humanitarian.total_collected || 0,
        humanitarian_distributed: humanitarian.total_distributed || 0,
        humanitarian_balance: humanitarian.balance || 0
      },
      balances_by_class: totalsByClass,
      classifications: Object.entries(MONETARY_CLASSIFICATIONS).map(([code, c]) => ({
        code,
        name: c.name,
        color: c.color,
        total_value: totalsByClass[code] || 0,
        holdings_count: activeHoldings.filter(h => h.mclass === code).length,
        humanitarian_percent: c.humanitarian_percent
      })),
      humanitarian_allocations: humanitarian.allocations || [],
      recent_conversions: recentConversions
    };
  });

  // ==================== RECEIVE FUNDS (M0-M4) ====================
  
  fastify.post('/api/monetary/receive', async (req) => {
    const data = db.get();
    const { 
      entity_type, 
      entity_id, 
      entity_name, 
      mclass, 
      amount_usd,
      assets // Array of { asset_type, quantity, value_usd }
    } = req.body;
    
    const classification = MONETARY_CLASSIFICATIONS[mclass?.toUpperCase()];
    if (!classification) return { error: 'Invalid monetary classification' };
    
    const holdingsCreated = [];
    let totalValue = 0;
    
    // If assets provided, create individual holdings
    if (assets && assets.length > 0) {
      for (const asset of assets) {
        const assetDef = classification.assets.find(a => a.type === asset.asset_type);
        if (!assetDef) continue;
        
        const holdingId = db.nextId('asset_holdings');
        const value = asset.value_usd || (asset.quantity * (assetDef.price_per_unit || assetDef.avg_value || 1));
        
        const holding = {
          id: holdingId,
          entity_type,
          entity_id,
          entity_name,
          mclass: mclass.toUpperCase(),
          mclass_name: classification.name,
          asset_type: asset.asset_type,
          asset_name: assetDef.name,
          quantity: parseFloat(asset.quantity),
          unit: assetDef.unit,
          value_usd: value,
          certificate_number: `CERT-${mclass}-${holdingId}-${Date.now()}`,
          status: 'active',
          created_at: db.now(),
          updated_at: db.now()
        };
        
        data.asset_holdings.push(holding);
        holdingsCreated.push(holding);
        totalValue += value;
      }
    } else if (amount_usd) {
      // Create a general holding for the amount
      const holdingId = db.nextId('asset_holdings');
      const holding = {
        id: holdingId,
        entity_type,
        entity_id,
        entity_name,
        mclass: mclass.toUpperCase(),
        mclass_name: classification.name,
        asset_type: 'GENERAL',
        asset_name: `${classification.name} Holding`,
        quantity: 1,
        unit: 'lot',
        value_usd: parseFloat(amount_usd),
        certificate_number: `CERT-${mclass}-${holdingId}-${Date.now()}`,
        status: 'active',
        created_at: db.now(),
        updated_at: db.now()
      };
      
      data.asset_holdings.push(holding);
      holdingsCreated.push(holding);
      totalValue = parseFloat(amount_usd);
    }
    
    // Update M-Class balance
    const balanceKey = `${entity_type}-${entity_id}`;
    if (!data.mclass_balances[balanceKey]) {
      data.mclass_balances[balanceKey] = { M0: 0, M1: 0, M2: 0, M3: 0, M4: 0 };
    }
    data.mclass_balances[balanceKey][mclass.toUpperCase()] += totalValue;
    
    db.save();
    
    return {
      ok: true,
      mclass: mclass.toUpperCase(),
      total_received: totalValue,
      holdings_created: holdingsCreated.length,
      holdings: holdingsCreated,
      conversion_info: {
        rate_to_m0: classification.conversion_rate,
        humanitarian_percent: classification.humanitarian_percent,
        potential_m0_value: totalValue * classification.conversion_rate,
        potential_humanitarian: totalValue * classification.conversion_rate * (classification.humanitarian_percent / 100)
      }
    };
  });
}
