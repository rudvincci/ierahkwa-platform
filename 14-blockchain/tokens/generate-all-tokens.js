const fs = require('fs');
const path = require('path');

// All 103 IGT Tokens with descriptions
const tokens = [
    // Government (1-40)
    { id: '01', symbol: 'IGT-PM', name: 'Prime Minister', category: 'Government', desc: 'Official token of the Prime Minister Office. Used for executive decisions, national policies, and government coordination.', uses: ['Executive orders', 'Policy voting', 'Cabinet decisions', 'National emergencies'] },
    { id: '02', symbol: 'IGT-MFA', name: 'Foreign Affairs', category: 'Government', desc: 'Ministry of Foreign Affairs token for international relations, treaties, and diplomatic services.', uses: ['Treaty signing', 'Diplomatic credentials', 'Visa services', 'International agreements'] },
    { id: '03', symbol: 'IGT-MFT', name: 'Finance', category: 'Government', desc: 'Ministry of Finance token for fiscal policy, taxation, and national budget management.', uses: ['Tax payments', 'Budget allocation', 'Fiscal policy', 'Financial audits'] },
    { id: '04', symbol: 'IGT-MJ', name: 'Justice', category: 'Government', desc: 'Ministry of Justice token for legal proceedings, court fees, and judicial services.', uses: ['Court fees', 'Legal documents', 'Case filing', 'Judicial records'] },
    { id: '05', symbol: 'IGT-MI', name: 'Interior', category: 'Government', desc: 'Ministry of Interior token for internal affairs, civil services, and domestic security.', uses: ['Civil registration', 'Internal permits', 'Security clearances', 'Emergency response'] },
    { id: '06', symbol: 'IGT-MD', name: 'Defense', category: 'Government', desc: 'Ministry of Defense token for national defense, military services, and security operations.', uses: ['Defense contracts', 'Military services', 'Security protocols', 'Defense procurement'] },
    { id: '07', symbol: 'IGT-BDET', name: 'Central Bank', category: 'Government', desc: 'BDET Central Bank token for monetary policy, currency reserves, and banking regulation.', uses: ['Monetary policy', 'Bank licensing', 'Currency reserves', 'Interest rates'] },
    { id: '08', symbol: 'IGT-NT', name: 'Treasury', category: 'Government', desc: 'National Treasury token for government funds, bonds, and financial instruments.', uses: ['Government bonds', 'Treasury bills', 'Public funds', 'Debt management'] },
    { id: '09', symbol: 'IGT-AG', name: 'Attorney General', category: 'Government', desc: 'Attorney General token for legal representation, prosecution, and government legal advice.', uses: ['Legal prosecution', 'Government litigation', 'Legal opinions', 'Criminal cases'] },
    { id: '10', symbol: 'IGT-SC', name: 'Supreme Court', category: 'Government', desc: 'Supreme Court token for constitutional matters, appeals, and highest judicial decisions.', uses: ['Constitutional cases', 'Final appeals', 'Judicial review', 'Legal precedents'] },
    { id: '11', symbol: 'IGT-MH', name: 'Health', category: 'Government', desc: 'Ministry of Health token for healthcare services, medical facilities, and public health programs.', uses: ['Hospital services', 'Medical records', 'Health insurance', 'Vaccination programs'] },
    { id: '12', symbol: 'IGT-ME', name: 'Education', category: 'Government', desc: 'Ministry of Education token for schools, universities, and educational programs.', uses: ['School enrollment', 'University fees', 'Scholarships', 'Certifications'] },
    { id: '13', symbol: 'IGT-MLE', name: 'Labor', category: 'Government', desc: 'Ministry of Labor token for employment services, worker rights, and labor regulations.', uses: ['Work permits', 'Labor disputes', 'Employment services', 'Worker benefits'] },
    { id: '14', symbol: 'IGT-MSD', name: 'Social Development', category: 'Government', desc: 'Ministry of Social Development token for welfare programs and social assistance.', uses: ['Welfare payments', 'Social programs', 'Community development', 'Poverty reduction'] },
    { id: '15', symbol: 'IGT-MHO', name: 'Housing', category: 'Government', desc: 'Ministry of Housing token for housing programs, urban development, and construction permits.', uses: ['Housing permits', 'Urban planning', 'Construction licenses', 'Public housing'] },
    { id: '16', symbol: 'IGT-MCH', name: 'Culture', category: 'Government', desc: 'Ministry of Culture token for cultural preservation, arts, and heritage protection.', uses: ['Cultural events', 'Heritage sites', 'Art grants', 'Museum access'] },
    { id: '17', symbol: 'IGT-MSR', name: 'Sports', category: 'Government', desc: 'Ministry of Sports token for athletic programs, sports facilities, and competitions.', uses: ['Sports events', 'Athlete registration', 'Facility access', 'Competition fees'] },
    { id: '18', symbol: 'IGT-MFC', name: 'Family', category: 'Government', desc: 'Ministry of Family token for family services, child welfare, and domestic support.', uses: ['Family benefits', 'Child support', 'Adoption services', 'Family counseling'] },
    { id: '19', symbol: 'IGT-SSA', name: 'Social Security', category: 'Government', desc: 'Social Security Administration token for pensions, retirement, and social insurance.', uses: ['Pension payments', 'Retirement benefits', 'Disability claims', 'Survivor benefits'] },
    { id: '20', symbol: 'IGT-PHS', name: 'Public Health', category: 'Government', desc: 'Public Health Service token for disease control, health emergencies, and prevention.', uses: ['Disease control', 'Health alerts', 'Prevention programs', 'Emergency response'] },
    { id: '21', symbol: 'IGT-MA', name: 'Agriculture', category: 'Government', desc: 'Ministry of Agriculture token for farming, food security, and agricultural development.', uses: ['Farm subsidies', 'Crop insurance', 'Food safety', 'Agricultural permits'] },
    { id: '22', symbol: 'IGT-MEN', name: 'Environment', category: 'Government', desc: 'Ministry of Environment token for environmental protection, conservation, and sustainability.', uses: ['Environmental permits', 'Conservation', 'Pollution control', 'Green initiatives'] },
    { id: '23', symbol: 'IGT-MEG', name: 'Energy', category: 'Government', desc: 'Ministry of Energy token for power generation, utilities, and energy policy.', uses: ['Energy licenses', 'Utility payments', 'Power generation', 'Renewable energy'] },
    { id: '24', symbol: 'IGT-MMR', name: 'Mining', category: 'Government', desc: 'Ministry of Mining token for mineral extraction, mining licenses, and resource management.', uses: ['Mining permits', 'Mineral rights', 'Resource extraction', 'Environmental bonds'] },
    { id: '25', symbol: 'IGT-MCT', name: 'Commerce', category: 'Government', desc: 'Ministry of Commerce token for trade, business licensing, and commercial regulations.', uses: ['Business licenses', 'Trade permits', 'Commercial registration', 'Import/Export'] },
    { id: '26', symbol: 'IGT-MIN', name: 'Industry', category: 'Government', desc: 'Ministry of Industry token for manufacturing, industrial development, and production.', uses: ['Industrial permits', 'Factory licenses', 'Manufacturing', 'Quality standards'] },
    { id: '27', symbol: 'IGT-MT', name: 'Tourism', category: 'Government', desc: 'Ministry of Tourism token for travel services, hospitality, and tourist attractions.', uses: ['Tourism licenses', 'Hotel ratings', 'Tour operators', 'Tourist visas'] },
    { id: '28', symbol: 'IGT-MTR', name: 'Transport', category: 'Government', desc: 'Ministry of Transport token for transportation, roads, and public transit.', uses: ['Vehicle registration', 'Transit passes', 'Road tolls', 'Transport licenses'] },
    { id: '29', symbol: 'IGT-MST', name: 'Science', category: 'Government', desc: 'Ministry of Science token for research, technology, and scientific development.', uses: ['Research grants', 'Patents', 'Scientific projects', 'Innovation funding'] },
    { id: '30', symbol: 'IGT-MC', name: 'Communications', category: 'Government', desc: 'Ministry of Communications token for telecom, media, and information services.', uses: ['Telecom licenses', 'Media permits', 'Spectrum allocation', 'Broadcasting'] },
    { id: '31', symbol: 'IGT-NPS', name: 'Police', category: 'Government', desc: 'National Police Service token for law enforcement, public safety, and crime prevention.', uses: ['Police services', 'Background checks', 'Security clearances', 'Crime reports'] },
    { id: '32', symbol: 'IGT-AFI', name: 'Armed Forces', category: 'Government', desc: 'Armed Forces token for military operations, defense services, and national security.', uses: ['Military contracts', 'Defense services', 'Veterans benefits', 'Security operations'] },
    { id: '33', symbol: 'IGT-NIS', name: 'Intelligence', category: 'Government', desc: 'National Intelligence Service token for intelligence operations and national security.', uses: ['Intelligence services', 'Security clearances', 'Counter-terrorism', 'Surveillance'] },
    { id: '34', symbol: 'IGT-CBP', name: 'Customs', category: 'Government', desc: 'Customs and Border Protection token for import/export, border control, and tariffs.', uses: ['Customs duties', 'Import permits', 'Border crossing', 'Tariff payments'] },
    { id: '35', symbol: 'IGT-CRO', name: 'Civil Registry', category: 'Government', desc: 'Civil Registry Office token for birth, death, marriage, and civil status records.', uses: ['Birth certificates', 'Marriage licenses', 'Death records', 'Name changes'] },
    { id: '36', symbol: 'IGT-EC', name: 'Electoral', category: 'Government', desc: 'Electoral Commission token for voting, elections, and democratic processes.', uses: ['Voter registration', 'Election voting', 'Candidate registration', 'Electoral disputes'] },
    { id: '37', symbol: 'IGT-OCG', name: 'Comptroller', category: 'Government', desc: 'Office of Comptroller General token for government audits and financial oversight.', uses: ['Government audits', 'Financial oversight', 'Compliance checks', 'Public accounts'] },
    { id: '38', symbol: 'IGT-OO', name: 'Ombudsman', category: 'Government', desc: 'Ombudsman Office token for citizen complaints and government accountability.', uses: ['Citizen complaints', 'Government accountability', 'Rights protection', 'Public grievances'] },
    { id: '39', symbol: 'IGT-NA', name: 'Archives', category: 'Government', desc: 'National Archives token for historical records, documents, and preservation.', uses: ['Document access', 'Historical records', 'Archive storage', 'Record certification'] },
    { id: '40', symbol: 'IGT-PSI', name: 'Postal', category: 'Government', desc: 'Postal Service token for mail, packages, and postal services.', uses: ['Mail services', 'Package delivery', 'Postal money orders', 'P.O. Box rentals'] },
    
    // Finance (41-52)
    { id: '41', symbol: 'IGT-MAIN', name: 'Main Currency', category: 'Finance', desc: 'Primary sovereign currency token for all economic transactions within Ierahkwa.', uses: ['Daily transactions', 'Store of value', 'Unit of account', 'Medium of exchange'] },
    { id: '42', symbol: 'IGT-STABLE', name: 'Stablecoin', category: 'Finance', desc: 'Price-stable token pegged to basket of assets for low-volatility transactions.', uses: ['Stable payments', 'Remittances', 'Trading pairs', 'DeFi collateral'] },
    { id: '43', symbol: 'IGT-GOV', name: 'Governance', category: 'Finance', desc: 'Governance token for voting on protocol changes and platform decisions.', uses: ['Protocol voting', 'Proposal creation', 'Parameter changes', 'Treasury allocation'] },
    { id: '44', symbol: 'IGT-STAKE', name: 'Staking', category: 'Finance', desc: 'Staking rewards token earned by securing the network through proof-of-stake.', uses: ['Network staking', 'Validator rewards', 'Delegation', 'Network security'] },
    { id: '45', symbol: 'IGT-LIQ', name: 'Liquidity', category: 'Finance', desc: 'Liquidity provider token for DEX pools and automated market makers.', uses: ['Liquidity mining', 'LP rewards', 'Pool shares', 'AMM trading'] },
    { id: '46', symbol: 'IGT-REWARD', name: 'Rewards', category: 'Finance', desc: 'Reward distribution token for incentivizing platform participation.', uses: ['User rewards', 'Referral bonuses', 'Activity incentives', 'Loyalty programs'] },
    { id: '47', symbol: 'IGT-FEE', name: 'Fee Token', category: 'Finance', desc: 'Fee payment token for reduced transaction costs across the ecosystem.', uses: ['Transaction fees', 'Gas payments', 'Fee discounts', 'Protocol fees'] },
    { id: '48', symbol: 'IGT-BRIDGE', name: 'Bridge', category: 'Finance', desc: 'Cross-chain bridge token for transferring assets between blockchains.', uses: ['Cross-chain transfers', 'Bridge liquidity', 'Multi-chain access', 'Asset wrapping'] },
    { id: '49', symbol: 'IGT-RESERVE', name: 'Reserve', category: 'Finance', desc: 'Reserve backing token for maintaining stablecoin and currency reserves.', uses: ['Reserve backing', 'Collateral', 'Stability fund', 'Emergency reserves'] },
    { id: '50', symbol: 'IGT-TRADE', name: 'Trade', category: 'Finance', desc: 'Trading platform token for exchange services and trading fee discounts.', uses: ['Trading fees', 'VIP tiers', 'Margin trading', 'Futures access'] },
    { id: '51', symbol: 'IGT-DEFI', name: 'DeFi', category: 'Finance', desc: 'Decentralized finance token for lending, borrowing, and yield farming.', uses: ['Lending', 'Borrowing', 'Yield farming', 'Flash loans'] },
    { id: '52', symbol: 'IGT-ASSET', name: 'Asset', category: 'Finance', desc: 'Asset tokenization for real-world assets on the blockchain.', uses: ['Real estate tokens', 'Commodity tokens', 'Securities', 'Fractional ownership'] },
    
    // Services (53-103)
    { id: '53', symbol: 'IGT-EXCHANGE', name: 'Exchange', category: 'Services', desc: 'TradeX Exchange platform token for trading services and exchange features.', uses: ['Exchange access', 'Trading tools', 'Market making', 'Order books'] },
    { id: '54', symbol: 'IGT-TRADING', name: 'Trading', category: 'Services', desc: 'Advanced trading features token for professional traders and algorithms.', uses: ['API access', 'Bot trading', 'Advanced charts', 'Trading signals'] },
    { id: '55', symbol: 'IGT-CASINO', name: 'Casino', category: 'Services', desc: 'Casino gaming token for online gambling, slots, and table games.', uses: ['Casino games', 'Sports betting', 'Poker', 'Jackpots'] },
    { id: '56', symbol: 'IGT-SOCIAL', name: 'Social', category: 'Services', desc: 'Social media platform token for content creation and community engagement.', uses: ['Content rewards', 'Tipping', 'Premium features', 'Creator monetization'] },
    { id: '57', symbol: 'IGT-LOTTO', name: 'Lottery', category: 'Services', desc: 'Lottery and raffle token for prize drawings and lucky games.', uses: ['Lottery tickets', 'Raffles', 'Prize pools', 'Lucky draws'] },
    { id: '58', symbol: 'IGT-GLOBAL', name: 'Global Service', category: 'Services', desc: 'Global services access token for international banking and transfers.', uses: ['SWIFT transfers', 'International banking', 'Global payments', 'Currency exchange'] },
    { id: '59', symbol: 'IGT-NET', name: 'Network', category: 'Services', desc: 'Network infrastructure token for node operations and connectivity.', uses: ['Node rewards', 'Bandwidth', 'Network access', 'Infrastructure'] },
    { id: '60', symbol: 'IGT-SWIFT', name: 'SWIFT', category: 'Services', desc: 'International transfer token compatible with SWIFT banking network.', uses: ['Bank transfers', 'Wire transfers', 'International settlements', 'Correspondent banking'] },
    { id: '61', symbol: 'IGT-CLEAR', name: 'Clearing', category: 'Services', desc: 'Clearinghouse token for settlement and clearing of financial transactions.', uses: ['Trade settlement', 'Clearing services', 'Netting', 'Risk management'] },
    { id: '62', symbol: 'IGT-PAY', name: 'Payment', category: 'Services', desc: 'Payment processing token for merchant services and point-of-sale.', uses: ['POS payments', 'Merchant services', 'Payment gateway', 'QR payments'] },
    { id: '63', symbol: 'IGT-WALLET', name: 'Wallet', category: 'Services', desc: 'Digital wallet token for secure storage and transaction management.', uses: ['Wallet services', 'Multi-sig', 'Cold storage', 'Key management'] },
    { id: '64', symbol: 'IGT-INSURANCE', name: 'Insurance', category: 'Services', desc: 'Insurance services token for coverage, claims, and risk protection.', uses: ['Insurance premiums', 'Claims processing', 'Coverage', 'Risk pools'] },
    { id: '65', symbol: 'IGT-LOANS', name: 'Loans', category: 'Services', desc: 'Lending platform token for personal, business, and crypto-backed loans.', uses: ['Personal loans', 'Business loans', 'Crypto loans', 'Mortgage'] },
    { id: '66', symbol: 'IGT-MARKET', name: 'Marketplace', category: 'Services', desc: 'General marketplace token for buying and selling goods and services.', uses: ['E-commerce', 'Auctions', 'Classifieds', 'P2P trading'] },
    { id: '67', symbol: 'IGT-HEALTH', name: 'Healthcare', category: 'Services', desc: 'Healthcare services token for medical appointments and health records.', uses: ['Medical services', 'Health records', 'Telemedicine', 'Prescriptions'] },
    { id: '68', symbol: 'IGT-EDU', name: 'Education', category: 'Services', desc: 'Educational platform token for courses, certifications, and learning.', uses: ['Online courses', 'Certifications', 'Tutoring', 'E-learning'] },
    { id: '69', symbol: 'IGT-TRAVEL', name: 'Travel', category: 'Services', desc: 'Travel booking token for flights, hotels, and vacation packages.', uses: ['Flight booking', 'Hotel reservations', 'Travel packages', 'Car rentals'] },
    { id: '70', symbol: 'IGT-SHIP', name: 'Shipping', category: 'Services', desc: 'Shipping and logistics token for package delivery and freight.', uses: ['Package shipping', 'Freight', 'Tracking', 'Logistics'] },
    { id: '71', symbol: 'IGT-CLOUD', name: 'Cloud', category: 'Services', desc: 'Cloud computing token for storage, hosting, and computing services.', uses: ['Cloud storage', 'Web hosting', 'Computing power', 'CDN services'] },
    { id: '72', symbol: 'IGT-AI', name: 'AI Services', category: 'Services', desc: 'Artificial intelligence token for AI services, ML models, and automation.', uses: ['AI models', 'Machine learning', 'Automation', 'Data analysis'] },
    { id: '73', symbol: 'IGT-VPN', name: 'VPN', category: 'Services', desc: 'VPN and privacy token for secure, anonymous internet access.', uses: ['VPN access', 'Privacy', 'Secure browsing', 'Anonymous traffic'] },
    { id: '74', symbol: 'IGT-STREAM', name: 'Streaming', category: 'Services', desc: 'Streaming platform token for video, music, and live content.', uses: ['Video streaming', 'Music streaming', 'Live streams', 'Content access'] },
    { id: '75', symbol: 'IGT-GAMING', name: 'Gaming', category: 'Services', desc: 'Gaming platform token for video games, esports, and in-game items.', uses: ['Game purchases', 'In-game items', 'Esports', 'Gaming rewards'] },
    { id: '76', symbol: 'IGT-MUSIC', name: 'Music', category: 'Services', desc: 'Music industry token for artists, royalties, and music distribution.', uses: ['Music royalties', 'Artist payments', 'Album releases', 'Concert tickets'] },
    { id: '77', symbol: 'IGT-NEWS', name: 'News', category: 'Services', desc: 'News and media token for journalism, subscriptions, and content.', uses: ['News subscriptions', 'Journalism', 'Media access', 'Content creation'] },
    { id: '78', symbol: 'IGT-SPORTS', name: 'Sports', category: 'Services', desc: 'Sports platform token for events, betting, and fan engagement.', uses: ['Event tickets', 'Sports betting', 'Fan tokens', 'Merchandise'] },
    { id: '79', symbol: 'IGT-REALTY', name: 'Real Estate', category: 'Services', desc: 'Real estate token for property listings, rentals, and transactions.', uses: ['Property sales', 'Rentals', 'Real estate investment', 'Property management'] },
    { id: '80', symbol: 'IGT-AUTO', name: 'Automotive', category: 'Services', desc: 'Automotive services token for car sales, rentals, and maintenance.', uses: ['Car purchases', 'Car rentals', 'Maintenance', 'Auto insurance'] },
    { id: '81', symbol: 'IGT-ENERGY', name: 'Energy', category: 'Services', desc: 'Energy trading token for electricity, gas, and renewable energy.', uses: ['Energy trading', 'Utility payments', 'Solar credits', 'Carbon offsets'] },
    { id: '82', symbol: 'IGT-TELECOM', name: 'Telecom', category: 'Services', desc: 'Telecommunications token for mobile, internet, and phone services.', uses: ['Mobile plans', 'Internet service', 'Phone calls', 'Data packages'] },
    { id: '83', symbol: 'IGT-MAIL', name: 'Mail', category: 'Services', desc: 'Digital mail and messaging token for secure communications.', uses: ['Email services', 'Secure messaging', 'Digital mail', 'Notifications'] },
    { id: '84', symbol: 'IGT-FOOD', name: 'Food', category: 'Services', desc: 'Food delivery and restaurant token for dining and delivery services.', uses: ['Food delivery', 'Restaurant orders', 'Meal subscriptions', 'Groceries'] },
    { id: '85', symbol: 'IGT-RIDE', name: 'Ride', category: 'Services', desc: 'Ride-sharing and transportation token for taxi and ride services.', uses: ['Taxi rides', 'Ride-sharing', 'Bike rentals', 'Scooter rentals'] },
    { id: '86', symbol: 'IGT-JOBS', name: 'Jobs', category: 'Services', desc: 'Job marketplace token for employment, freelancing, and recruitment.', uses: ['Job listings', 'Freelancing', 'Recruitment', 'Resume services'] },
    { id: '87', symbol: 'IGT-DATING', name: 'Dating', category: 'Services', desc: 'Dating platform token for matchmaking and relationship services.', uses: ['Dating profiles', 'Matchmaking', 'Premium features', 'Virtual gifts'] },
    { id: '88', symbol: 'IGT-HOTEL', name: 'Hotels', category: 'Services', desc: 'Hotel booking token for accommodations and hospitality.', uses: ['Hotel bookings', 'Hostel stays', 'Resort access', 'Loyalty rewards'] },
    { id: '89', symbol: 'IGT-FLIGHTS', name: 'Flights', category: 'Services', desc: 'Flight booking token for airline tickets and travel rewards.', uses: ['Flight tickets', 'Miles/points', 'Upgrades', 'Lounge access'] },
    { id: '90', symbol: 'IGT-LEGAL', name: 'Legal', category: 'Services', desc: 'Legal services token for attorneys, contracts, and legal documents.', uses: ['Legal consultations', 'Contract drafting', 'Court filings', 'Legal documents'] },
    { id: '91', symbol: 'IGT-ID', name: 'Identity', category: 'Services', desc: 'Digital identity token for KYC, verification, and identity management.', uses: ['KYC verification', 'Digital ID', 'Identity proofs', 'Access control'] },
    { id: '92', symbol: 'IGT-VOTE', name: 'Voting', category: 'Services', desc: 'Electronic voting token for secure, transparent democratic processes.', uses: ['E-voting', 'Polls', 'Surveys', 'Decision making'] },
    { id: '93', symbol: 'IGT-CHARITY', name: 'Charity', category: 'Services', desc: 'Charitable donation token for nonprofits and humanitarian causes.', uses: ['Donations', 'Charity campaigns', 'Disaster relief', 'Social causes'] },
    { id: '94', symbol: 'IGT-CROWDFUND', name: 'Crowdfunding', category: 'Services', desc: 'Crowdfunding token for project funding and community investments.', uses: ['Project funding', 'Startup investment', 'Community projects', 'Rewards'] },
    { id: '95', symbol: 'IGT-METAVERSE', name: 'Metaverse', category: 'Services', desc: 'Metaverse platform token for virtual worlds, land, and digital assets.', uses: ['Virtual land', 'Avatars', 'Virtual events', 'Digital fashion'] },
    { id: '96', symbol: 'IGT-NFT', name: 'NFT', category: 'Services', desc: 'NFT marketplace token for digital art, collectibles, and unique assets.', uses: ['NFT minting', 'Art sales', 'Collectibles', 'Royalties'] },
    { id: '97', symbol: 'IGT-LAUNCHPAD', name: 'Launchpad', category: 'Services', desc: 'Token launchpad for new project IDOs and token sales.', uses: ['IDO participation', 'Token sales', 'Early access', 'Project launches'] },
    { id: '98', symbol: 'IGT-DAO', name: 'DAO', category: 'Services', desc: 'DAO governance token for decentralized autonomous organizations.', uses: ['DAO voting', 'Proposals', 'Treasury management', 'Community governance'] },
    { id: '99', symbol: 'IGT-ORACLE', name: 'Oracle', category: 'Services', desc: 'Oracle network token for off-chain data feeds and price oracles.', uses: ['Price feeds', 'Data oracles', 'API access', 'External data'] },
    { id: '100', symbol: 'IGT-SOVEREIGN', name: 'Sovereign', category: 'Services', desc: 'Ultimate sovereignty token representing full citizenship and rights.', uses: ['Full citizenship', 'All services access', 'Governance rights', 'Premium benefits'] },
    { id: '101', symbol: 'IGT-IISB', name: 'Settlement', category: 'Services', desc: 'International settlement token for cross-border banking settlements.', uses: ['International settlements', 'Bank reconciliation', 'Cross-border payments', 'Clearing'] },
    { id: '102', symbol: 'IGT-DOCFLOW', name: 'Documents', category: 'Services', desc: 'Document management token for digital documents and workflows.', uses: ['Document storage', 'Workflow automation', 'Digital signatures', 'Version control'] },
    { id: '103', symbol: 'IGT-ESIGN', name: 'E-Signature', category: 'Services', desc: 'Electronic signature token for legally binding digital signatures.', uses: ['E-signatures', 'Contract signing', 'Document verification', 'Legal validity'] }
];

// HTML Template
const template = (token) => `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${token.symbol} - ${token.name} | Ierahkwa Government Token</title>
    <link href="https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700;800;900&family=Exo+2:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        :root {
            --gold: #FFD700;
            --gold-dark: #B8860B;
            --neon-green: #00FF41;
            --neon-cyan: #00FFFF;
            --neon-magenta: #FF00FF;
            --bg-dark: #0a0e17;
            --bg-card: #0d1a2d;
            --border: #1e3a5f;
        }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            font-family: 'Exo 2', sans-serif;
            background: linear-gradient(135deg, #0a0e17 0%, #0d1a2d 50%, #1a0a2e 100%);
            color: #fff;
            min-height: 100vh;
        }
        .bg-animation {
            position: fixed;
            top: 0; left: 0;
            width: 100%; height: 100%;
            pointer-events: none;
            z-index: 0;
            background: 
                radial-gradient(circle at 20% 80%, rgba(255, 215, 0, 0.05) 0%, transparent 50%),
                radial-gradient(circle at 80% 20%, rgba(0, 255, 65, 0.05) 0%, transparent 50%);
        }
        .container { position: relative; z-index: 1; max-width: 1200px; margin: 0 auto; padding: 40px 20px; }
        .back-link {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            color: var(--neon-cyan);
            text-decoration: none;
            font-family: 'Orbitron', sans-serif;
            margin-bottom: 30px;
            transition: all 0.3s;
        }
        .back-link:hover { color: var(--gold); }
        .token-header {
            display: flex;
            align-items: center;
            gap: 30px;
            margin-bottom: 40px;
            flex-wrap: wrap;
        }
        .token-icon {
            width: 120px;
            height: 120px;
            background: linear-gradient(135deg, var(--gold), var(--gold-dark));
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Orbitron', sans-serif;
            font-size: 1.2em;
            font-weight: 900;
            color: #000;
            box-shadow: 0 0 40px rgba(255, 215, 0, 0.5);
            animation: glow 2s ease-in-out infinite alternate;
        }
        @keyframes glow {
            from { box-shadow: 0 0 30px rgba(255, 215, 0, 0.5); }
            to { box-shadow: 0 0 60px rgba(255, 215, 0, 0.8); }
        }
        .token-info h1 {
            font-family: 'Orbitron', sans-serif;
            font-size: 2.5em;
            background: linear-gradient(90deg, var(--gold), var(--neon-green));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            margin-bottom: 10px;
        }
        .token-info .symbol {
            font-family: 'Orbitron', sans-serif;
            font-size: 1.5em;
            color: var(--neon-cyan);
        }
        .token-info .category {
            display: inline-block;
            background: rgba(0, 255, 65, 0.15);
            color: var(--neon-green);
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 0.9em;
            margin-top: 10px;
        }
        .card {
            background: var(--bg-card);
            border: 2px solid var(--border);
            border-radius: 20px;
            padding: 30px;
            margin-bottom: 30px;
        }
        .card:hover { border-color: var(--gold); }
        .card h2 {
            font-family: 'Orbitron', sans-serif;
            color: var(--gold);
            margin-bottom: 20px;
            font-size: 1.3em;
        }
        .description {
            font-size: 1.1em;
            line-height: 1.8;
            color: #ccc;
        }
        .uses-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
        }
        .use-item {
            background: rgba(0, 255, 255, 0.1);
            border: 1px solid var(--border);
            border-radius: 12px;
            padding: 15px 20px;
            display: flex;
            align-items: center;
            gap: 12px;
            transition: all 0.3s;
        }
        .use-item:hover {
            border-color: var(--neon-cyan);
            transform: translateX(5px);
        }
        .use-item i { color: var(--neon-green); font-size: 1.2em; }
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 20px;
        }
        .stat-box {
            text-align: center;
            padding: 20px;
            background: rgba(255, 215, 0, 0.1);
            border-radius: 15px;
            border: 1px solid rgba(255, 215, 0, 0.3);
        }
        .stat-value {
            font-family: 'Orbitron', sans-serif;
            font-size: 1.8em;
            color: var(--gold);
        }
        .stat-label { color: #888; font-size: 0.9em; margin-top: 5px; }
        .buttons {
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
            margin-top: 30px;
        }
        .btn {
            padding: 15px 30px;
            border: none;
            border-radius: 12px;
            font-family: 'Orbitron', sans-serif;
            font-weight: 700;
            cursor: pointer;
            transition: all 0.3s;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 10px;
        }
        .btn:hover { transform: translateY(-3px); }
        .btn-gold { background: linear-gradient(135deg, var(--gold), var(--gold-dark)); color: #000; }
        .btn-gold:hover { box-shadow: 0 10px 30px rgba(255, 215, 0, 0.4); }
        .btn-green { background: linear-gradient(135deg, var(--neon-green), #00cc33); color: #000; }
        .btn-green:hover { box-shadow: 0 10px 30px rgba(0, 255, 65, 0.4); }
        .btn-cyan { background: linear-gradient(135deg, var(--neon-cyan), #0099cc); color: #000; }
        .btn-cyan:hover { box-shadow: 0 10px 30px rgba(0, 255, 255, 0.4); }
        .btn-outline { background: transparent; border: 2px solid var(--gold); color: var(--gold); }
        .btn-outline:hover { background: rgba(255, 215, 0, 0.1); }
        footer {
            text-align: center;
            padding: 30px;
            margin-top: 50px;
            border-top: 2px solid var(--border);
            color: #888;
        }
        footer .logo { font-family: 'Orbitron', sans-serif; color: var(--gold); margin-bottom: 10px; }
        @media (max-width: 768px) {
            .token-header { flex-direction: column; text-align: center; }
            .token-info h1 { font-size: 1.8em; }
        }
    </style>
</head>
<body>
    <div class="bg-animation"></div>
    <div class="container">
        <a href="../index.html" class="back-link"><i class="bi bi-arrow-left"></i> Back to Token Registry</a>
        
        <div class="token-header">
            <div class="token-icon">${token.symbol.replace('IGT-', '')}</div>
            <div class="token-info">
                <h1>${token.name}</h1>
                <div class="symbol">${token.symbol}</div>
                <span class="category">${token.category}</span>
            </div>
        </div>
        
        <div class="card">
            <h2><i class="bi bi-info-circle"></i> About This Token</h2>
            <p class="description">${token.desc}</p>
        </div>
        
        <div class="card">
            <h2><i class="bi bi-lightning"></i> Token Uses</h2>
            <div class="uses-grid">
                ${token.uses.map(use => `<div class="use-item"><i class="bi bi-check-circle-fill"></i>${use}</div>`).join('\n                ')}
            </div>
        </div>
        
        <div class="card">
            <h2><i class="bi bi-graph-up"></i> Token Statistics</h2>
            <div class="stats-grid">
                <div class="stat-box">
                    <div class="stat-value">10T</div>
                    <div class="stat-label">Total Supply</div>
                </div>
                <div class="stat-box">
                    <div class="stat-value">${token.id}</div>
                    <div class="stat-label">Token ID</div>
                </div>
                <div class="stat-box">
                    <div class="stat-value">ISB</div>
                    <div class="stat-label">Blockchain</div>
                </div>
                <div class="stat-box">
                    <div class="stat-value">18</div>
                    <div class="stat-label">Decimals</div>
                </div>
            </div>
        </div>
        
        <div class="buttons">
            <a href="http://localhost:5054" class="btn btn-gold"><i class="bi bi-currency-exchange"></i> Trade on TradeX</a>
            <a href="http://localhost:5071" class="btn btn-green"><i class="bi bi-water"></i> Add Liquidity</a>
            <a href="#" class="btn btn-cyan"><i class="bi bi-wallet2"></i> Add to Wallet</a>
            <a href="whitepaper.md" class="btn btn-outline"><i class="bi bi-file-text"></i> Whitepaper</a>
        </div>
        
        <footer>
            <div class="logo">🏛️ IERAHKWA FUTUREHEAD PLATFORM</div>
            <p>Sovereign Government of Ierahkwa Ne Kanienke<br>© 2026 All Rights Reserved</p>
        </footer>
    </div>
</body>
</html>`;

// Generate all token pages
const tokensDir = __dirname;

tokens.forEach(token => {
    const folderName = token.id + '-' + token.symbol;
    const folderPath = path.join(tokensDir, folderName);
    
    // Create folder if not exists
    if (!fs.existsSync(folderPath)) {
        fs.mkdirSync(folderPath, { recursive: true });
    }
    
    // Generate index.html
    const htmlPath = path.join(folderPath, 'index.html');
    fs.writeFileSync(htmlPath, template(token));
    
    // Generate token.json
    const jsonPath = path.join(folderPath, 'token.json');
    fs.writeFileSync(jsonPath, JSON.stringify({
        id: token.id,
        symbol: token.symbol,
        name: token.name,
        category: token.category,
        description: token.desc,
        uses: token.uses,
        totalSupply: "10000000000000",
        decimals: 18,
        blockchain: "ISB",
        chainId: 777777,
        contract: '0x' + token.id.padStart(40, '0')
    }, null, 2));
    
    console.log('✓ Generated: ' + folderName);
});

console.log('\n✅ Generated ' + tokens.length + ' token landing pages!');
