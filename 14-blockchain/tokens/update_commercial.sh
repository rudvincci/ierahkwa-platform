#!/bin/bash

BASE="/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/tokens"

create_commercial_page() {
    local folder=$1
    local id=$2
    local symbol=$3
    local name=$4
    local icon=$5
    local desc=$6
    local use1=$7
    local use2=$8
    local use3=$9
    local use4=${10}
    local use5=${11}
    local use6=${12}
    local industry=${13}
    
    mkdir -p "$BASE/$folder"
    
    cat > "$BASE/$folder/index.html" << EOF
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>$symbol | $name - Commercial Token</title>
    <link href="https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700;800;900&family=Exo+2:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <style>
        :root { --gold: #FFD700; --green: #00FF41; --cyan: #00FFFF; --purple: #9D4EDD; --bg: #0a0e17; }
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Exo 2', sans-serif; background: linear-gradient(135deg, #0a0e17 0%, #0d1a2d 50%, #1a0a2e 100%); color: #fff; min-height: 100vh; }
        .container { max-width: 1200px; margin: 0 auto; padding: 40px 20px; }
        .back-link { color: var(--cyan); text-decoration: none; display: flex; align-items: center; gap: 8px; margin-bottom: 30px; }
        .header { display: flex; align-items: center; gap: 30px; margin-bottom: 40px; flex-wrap: wrap; }
        .icon { width: 120px; height: 120px; background: linear-gradient(135deg, var(--gold), #B8860B); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 3em; box-shadow: 0 0 50px rgba(255, 215, 0, 0.5); }
        .info h1 { font-family: 'Orbitron', sans-serif; font-size: 2.2em; color: var(--gold); }
        .info .symbol { font-family: 'Orbitron', sans-serif; font-size: 1.3em; color: var(--cyan); }
        .badge { display: inline-block; background: linear-gradient(90deg, var(--green), #00aa33); color: #000; padding: 5px 15px; border-radius: 20px; font-size: 0.85em; font-weight: bold; margin-top: 10px; }
        .card { background: rgba(13, 26, 45, 0.9); border: 2px solid rgba(255, 215, 0, 0.3); border-radius: 20px; padding: 30px; margin-bottom: 25px; }
        .card:hover { border-color: var(--gold); }
        .card h2 { font-family: 'Orbitron', sans-serif; color: var(--gold); margin-bottom: 20px; font-size: 1.2em; }
        .desc { font-size: 1.1em; line-height: 1.8; color: #ccc; }
        .commerce-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 15px; }
        .commerce-item { background: rgba(0, 255, 65, 0.1); border: 1px solid rgba(0, 255, 65, 0.3); border-radius: 12px; padding: 20px; display: flex; align-items: center; gap: 15px; }
        .commerce-item:hover { border-color: var(--green); transform: translateX(5px); transition: all 0.3s; }
        .commerce-icon { font-size: 1.5em; }
        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 15px; }
        .stat { text-align: center; padding: 20px; background: rgba(255, 215, 0, 0.1); border-radius: 15px; border: 1px solid rgba(255, 215, 0, 0.3); }
        .stat-value { font-family: 'Orbitron', sans-serif; font-size: 1.6em; color: var(--gold); }
        .stat-label { color: #888; font-size: 0.85em; margin-top: 5px; }
        .buttons { display: flex; gap: 15px; flex-wrap: wrap; margin-top: 30px; }
        .btn { padding: 15px 25px; border: none; border-radius: 12px; font-family: 'Orbitron', sans-serif; font-weight: 700; cursor: pointer; text-decoration: none; display: inline-flex; align-items: center; gap: 10px; transition: all 0.3s; }
        .btn:hover { transform: translateY(-3px); }
        .btn-gold { background: linear-gradient(135deg, var(--gold), #B8860B); color: #000; }
        .btn-green { background: linear-gradient(135deg, var(--green), #00aa33); color: #000; }
        .btn-cyan { background: linear-gradient(135deg, var(--cyan), #0099cc); color: #000; }
        .highlight { background: linear-gradient(135deg, rgba(255, 215, 0, 0.15), rgba(0, 0, 0, 0.3)); border: 2px solid var(--gold); padding: 25px; border-radius: 15px; margin: 20px 0; }
        .highlight h3 { color: var(--gold); font-family: 'Orbitron', sans-serif; margin-bottom: 15px; }
        .highlight ul { list-style: none; }
        .highlight li { padding: 8px 0; color: #ddd; display: flex; align-items: center; gap: 10px; }
        .highlight li::before { content: '✓'; color: var(--green); font-weight: bold; }
        footer { text-align: center; padding: 30px; margin-top: 50px; border-top: 2px solid rgba(255, 215, 0, 0.2); color: #888; }
        .ecosystem { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-top: 20px; }
        .eco-item { background: rgba(157, 78, 221, 0.15); border: 1px solid rgba(157, 78, 221, 0.4); border-radius: 12px; padding: 15px; text-align: center; }
        .eco-item:hover { border-color: var(--purple); }
        .eco-icon { font-size: 2em; margin-bottom: 10px; }
        .eco-name { color: #fff; font-weight: bold; }
        .eco-desc { color: #888; font-size: 0.85em; }
    </style>
</head>
<body>
    <div class="container">
        <a href="../../platform/departments.html" class="back-link">← Back to 103 Departments</a>
        
        <div class="header">
            <div class="icon">$icon</div>
            <div class="info">
                <h1>$name</h1>
                <div class="symbol">$symbol</div>
                <span class="badge">🪙 COMMERCIAL TOKEN #$id</span>
            </div>
        </div>
        
        <div class="card">
            <h2>💼 ABOUT THIS COMMERCIAL TOKEN</h2>
            <p class="desc">$desc</p>
            
            <div class="highlight">
                <h3>🏪 WHAT YOU CAN BUY WITH $symbol</h3>
                <ul>
                    <li>Pay for goods and services in the $name sector</li>
                    <li>Access exclusive discounts at partner merchants</li>
                    <li>Stake tokens to earn rewards from transaction fees</li>
                    <li>Trade on TradeX Exchange for other IGT tokens</li>
                    <li>Use as collateral for loans in the DeFi ecosystem</li>
                </ul>
            </div>
        </div>
        
        <div class="card">
            <h2>🛒 COMMERCE & SERVICES</h2>
            <p style="color: #aaa; margin-bottom: 20px;">Accept and pay with $symbol in these commercial activities:</p>
            <div class="commerce-grid">
                <div class="commerce-item"><span class="commerce-icon">🏪</span><span>$use1</span></div>
                <div class="commerce-item"><span class="commerce-icon">💳</span><span>$use2</span></div>
                <div class="commerce-item"><span class="commerce-icon">📦</span><span>$use3</span></div>
                <div class="commerce-item"><span class="commerce-icon">🤝</span><span>$use4</span></div>
                <div class="commerce-item"><span class="commerce-icon">⚡</span><span>$use5</span></div>
                <div class="commerce-item"><span class="commerce-icon">🌐</span><span>$use6</span></div>
            </div>
        </div>
        
        <div class="card">
            <h2>📊 TOKEN ECONOMICS</h2>
            <div class="stats">
                <div class="stat"><div class="stat-value">10T</div><div class="stat-label">Total Supply</div></div>
                <div class="stat"><div class="stat-value">#$id</div><div class="stat-label">Token ID</div></div>
                <div class="stat"><div class="stat-value">ISB</div><div class="stat-label">Blockchain</div></div>
                <div class="stat"><div class="stat-value">0.1%</div><div class="stat-label">Tx Fee</div></div>
                <div class="stat"><div class="stat-value">18</div><div class="stat-label">Decimals</div></div>
                <div class="stat"><div class="stat-value">Active</div><div class="stat-label">Status</div></div>
            </div>
        </div>
        
        <div class="card">
            <h2>🌐 ECOSYSTEM INTEGRATION</h2>
            <p style="color: #aaa; margin-bottom: 15px;">$symbol integrates with the entire Ierahkwa financial ecosystem:</p>
            <div class="ecosystem">
                <div class="eco-item"><div class="eco-icon">💱</div><div class="eco-name">TradeX Exchange</div><div class="eco-desc">Trade $symbol pairs</div></div>
                <div class="eco-item"><div class="eco-icon">🏦</div><div class="eco-name">BDET Bank</div><div class="eco-desc">Savings & loans</div></div>
                <div class="eco-item"><div class="eco-icon">💧</div><div class="eco-name">Liquidity Pools</div><div class="eco-desc">Earn from fees</div></div>
                <div class="eco-item"><div class="eco-icon">🎰</div><div class="eco-name">IGT Casino</div><div class="eco-desc">Gaming accepted</div></div>
            </div>
        </div>
        
        <div class="buttons">
            <a href="http://localhost:5054" class="btn btn-gold">💱 Trade on TradeX</a>
            <a href="http://localhost:5071" class="btn btn-green">💧 Add Liquidity</a>
            <a href="#" class="btn btn-cyan">👛 Add to Wallet</a>
        </div>
        
        <footer>
            <div style="font-family: 'Orbitron', sans-serif; color: var(--gold); margin-bottom: 10px;">🪙 $symbol - COMMERCIAL TOKEN</div>
            <p>Sovereign Government of Ierahkwa Ne Kanienke<br>Part of the 103 IGT Token Ecosystem<br>© 2026 All Rights Reserved</p>
        </footer>
    </div>
</body>
</html>
EOF
    echo "Created: $folder ($symbol)"
}

# GOVERNMENT COMMERCE TOKENS (1-40) - Tokenized Industries
create_commercial_page "01-IGT-PM" "01" "IGT-PM" "Executive Services" "👑" "Premier token for executive and VIP services. Use IGT-PM to access premium government services, diplomatic facilities, official ceremonies, and executive business services across the sovereign nation." "VIP Event Access" "Diplomatic Services" "Executive Transport" "Premium Memberships" "Official Ceremonies" "Government Consulting"
create_commercial_page "02-IGT-MFA" "02" "IGT-MFA" "International Trade" "🌍" "Token for international commerce, import/export services, and cross-border transactions. Pay for customs clearance, international shipping, trade documentation, and global business services." "Import/Export Fees" "Trade Documentation" "International Shipping" "Customs Clearance" "Foreign Exchange" "Trade Consulting"
create_commercial_page "03-IGT-MFT" "03" "IGT-MFT" "Financial Services" "💰" "The primary financial services token. Use for banking fees, investment services, tax payments, financial consulting, accounting services, and all monetary transactions." "Banking Services" "Investment Products" "Tax Payments" "Financial Consulting" "Accounting Services" "Audit Services"
create_commercial_page "04-IGT-MJ" "04" "IGT-MJ" "Legal Services" "⚖️" "Token for legal and judicial services. Pay for legal representation, court fees, notary services, legal documentation, dispute resolution, and compliance consulting." "Legal Representation" "Court Fees" "Notary Services" "Contract Review" "Dispute Resolution" "Legal Consulting"
create_commercial_page "05-IGT-MI" "05" "IGT-MI" "Interior Commerce" "🏠" "Token for domestic commerce and interior services. Use for business licenses, permits, local commerce, property services, and domestic trade within the sovereign territory." "Business Licenses" "Permits & Approvals" "Property Services" "Local Commerce" "Domestic Trade" "Administrative Fees"
create_commercial_page "06-IGT-MD" "06" "IGT-MD" "Security Services" "🛡️" "Token for private security, defense contracting, and protection services. Pay for security consulting, private protection, surveillance systems, and safety equipment." "Private Security" "Security Systems" "Defense Equipment" "Protection Services" "Safety Consulting" "Surveillance Tech"
create_commercial_page "07-IGT-BDET" "07" "IGT-BDET" "Central Banking" "🏦" "The central bank utility token. Use for interbank transfers, reserve operations, monetary policy services, and wholesale banking between financial institutions." "Interbank Transfers" "Reserve Operations" "Settlement Services" "Wholesale Banking" "Currency Exchange" "Monetary Services"
create_commercial_page "08-IGT-NT" "08" "IGT-NT" "Treasury & Bonds" "💵" "Token for government bonds, treasury bills, and sovereign debt instruments. Invest in national bonds, purchase treasury securities, and access fixed-income products." "Government Bonds" "Treasury Bills" "Sovereign Debt" "Fixed Income" "Bond Trading" "Interest Payments"
create_commercial_page "09-IGT-AG" "09" "IGT-AG" "Prosecution Services" "👨‍⚖️" "Token for prosecution, investigation, and enforcement services. Pay for legal investigations, compliance audits, enforcement actions, and regulatory services." "Legal Investigations" "Compliance Audits" "Enforcement Services" "Regulatory Filing" "Due Diligence" "Investigation Fees"
create_commercial_page "10-IGT-SC" "10" "IGT-SC" "Court Services" "🏛️" "Token for supreme court and appellate services. Access high-level judicial proceedings, appeals, constitutional matters, and premium legal arbitration." "Appeals Processing" "Constitutional Cases" "Arbitration Services" "Judicial Review" "Expert Testimony" "Legal Archives"
create_commercial_page "11-IGT-MH" "11" "IGT-MH" "Healthcare Commerce" "🏥" "The healthcare industry token. Pay for hospitals, clinics, pharmacies, medical equipment, health insurance, telemedicine, and all healthcare services." "Hospital Services" "Pharmacy Purchases" "Medical Equipment" "Health Insurance" "Telemedicine" "Lab Services"
create_commercial_page "12-IGT-ME" "12" "IGT-ME" "Education Services" "📚" "Token for educational services and institutions. Pay for tuition, courses, certifications, educational materials, e-learning platforms, and academic services." "Tuition Payments" "Online Courses" "Certifications" "Educational Materials" "E-Learning Access" "Academic Services"
create_commercial_page "13-IGT-MLE" "13" "IGT-MLE" "Employment Services" "👷" "Token for labor and employment services. Use for job placement, recruitment, HR services, training programs, workplace consulting, and staffing agencies." "Job Placement" "Recruitment Services" "HR Consulting" "Training Programs" "Staffing Agencies" "Career Services"
create_commercial_page "14-IGT-MSD" "14" "IGT-MSD" "Social Services" "🤝" "Token for social welfare and community services. Access social programs, community assistance, charitable services, NGO partnerships, and social development." "Social Programs" "Community Services" "Charitable Donations" "NGO Partnerships" "Welfare Access" "Social Support"
create_commercial_page "15-IGT-MHO" "15" "IGT-MHO" "Housing & Real Estate" "🏘️" "Token for real estate and housing services. Buy, sell, rent properties, pay for mortgages, property management, construction, and real estate services." "Property Sales" "Rental Payments" "Mortgage Services" "Construction" "Property Management" "Real Estate Fees"
create_commercial_page "16-IGT-MCH" "16" "IGT-MCH" "Culture & Entertainment" "🎭" "Token for cultural and entertainment industry. Pay for theaters, museums, art galleries, cultural events, entertainment venues, and creative services." "Theater Tickets" "Museum Access" "Art Purchases" "Cultural Events" "Entertainment Venues" "Creative Services"
create_commercial_page "17-IGT-MSR" "17" "IGT-MSR" "Sports Industry" "⚽" "Token for sports and recreation industry. Pay for sporting events, fitness centers, sports equipment, athletic training, and recreational facilities." "Event Tickets" "Gym Memberships" "Sports Equipment" "Athletic Training" "Recreation Facilities" "Sports Betting"
create_commercial_page "18-IGT-MFC" "18" "IGT-MFC" "Family Services" "👨‍👩‍👧" "Token for family and childcare services. Pay for daycare, family counseling, adoption services, child education, family entertainment, and support services." "Daycare Services" "Family Counseling" "Childcare" "Family Activities" "Parenting Services" "Support Programs"
create_commercial_page "19-IGT-SSA" "19" "IGT-SSA" "Insurance & Pensions" "🛡️" "Token for social security, insurance, and retirement services. Pay premiums, access retirement funds, claim benefits, and manage insurance products." "Insurance Premiums" "Pension Contributions" "Retirement Funds" "Benefit Claims" "Life Insurance" "Disability Coverage"
create_commercial_page "20-IGT-PHS" "20" "IGT-PHS" "Public Health" "💊" "Token for public health services and wellness. Pay for vaccinations, health screenings, wellness programs, preventive care, and public health initiatives." "Vaccinations" "Health Screenings" "Wellness Programs" "Preventive Care" "Health Education" "Sanitization Services"
create_commercial_page "21-IGT-MA" "21" "IGT-MA" "Agriculture Commerce" "🌾" "Token for agricultural industry. Buy/sell crops, livestock, farming equipment, agricultural services, food processing, and farm-to-table commerce." "Crop Trading" "Livestock Sales" "Farm Equipment" "Agricultural Services" "Food Processing" "Organic Products"
create_commercial_page "22-IGT-MEN" "22" "IGT-MEN" "Environmental Services" "🌿" "Token for environmental and green services. Pay for recycling, renewable energy credits, carbon offsets, eco-consulting, and environmental protection." "Recycling Services" "Carbon Credits" "Green Consulting" "Eco-Certification" "Waste Management" "Conservation"
create_commercial_page "23-IGT-MEG" "23" "IGT-MEG" "Energy Sector" "⚡" "Token for energy industry. Pay for electricity, natural gas, solar energy, battery storage, energy consulting, and utility services." "Electricity Bills" "Solar Energy" "Natural Gas" "Energy Storage" "Utility Services" "Energy Consulting"
create_commercial_page "24-IGT-MMR" "24" "IGT-MMR" "Mining & Resources" "⛏️" "Token for mining and natural resources industry. Trade minerals, precious metals, mining equipment, extraction services, and resource exploration." "Mineral Trading" "Precious Metals" "Mining Equipment" "Extraction Services" "Resource Exploration" "Geological Surveys"
create_commercial_page "25-IGT-MCT" "25" "IGT-MCT" "Retail Commerce" "🛒" "The primary retail commerce token. Use at stores, shopping centers, e-commerce platforms, wholesale markets, and all retail transactions." "Retail Purchases" "E-Commerce" "Wholesale Trading" "Shopping Centers" "Marketplace Fees" "Merchant Services"
create_commercial_page "26-IGT-MIN" "26" "IGT-MIN" "Industrial Manufacturing" "🏭" "Token for industrial and manufacturing sector. Pay for factory services, industrial equipment, manufacturing contracts, and B2B industrial commerce." "Manufacturing" "Industrial Equipment" "Factory Services" "B2B Commerce" "Production Contracts" "Quality Control"
create_commercial_page "27-IGT-MT" "27" "IGT-MT" "Tourism Industry" "🏝️" "Token for tourism and hospitality. Pay for tours, attractions, travel packages, vacation rentals, and all tourism-related services." "Tour Packages" "Attractions" "Vacation Rentals" "Travel Guides" "Excursions" "Tourism Services"
create_commercial_page "28-IGT-MTR" "28" "IGT-MTR" "Transportation" "🚂" "Token for transportation and logistics. Pay for public transit, freight shipping, vehicle rentals, logistics services, and transportation infrastructure." "Public Transit" "Freight Shipping" "Vehicle Rentals" "Logistics Services" "Parking Fees" "Transport Passes"
create_commercial_page "29-IGT-MST" "29" "IGT-MST" "Tech & Innovation" "🔬" "Token for science, technology, and R&D services. Pay for research, tech consulting, lab services, patent filings, and innovation programs." "Research Services" "Tech Consulting" "Lab Services" "Patent Filings" "R&D Contracts" "Innovation Programs"
create_commercial_page "30-IGT-MC" "30" "IGT-MC" "Telecommunications" "📡" "Token for communications and media industry. Pay for phone services, internet, broadcasting, media production, and telecommunications." "Phone Services" "Internet Plans" "Broadcasting" "Media Production" "Streaming Services" "Telecom Equipment"
create_commercial_page "31-IGT-NPS" "31" "IGT-NPS" "Security Commerce" "🚔" "Token for private police and security services. Pay for security guards, patrol services, alarm systems, background checks, and safety consulting." "Security Guards" "Patrol Services" "Alarm Systems" "Background Checks" "Safety Consulting" "Emergency Response"
create_commercial_page "32-IGT-AFI" "32" "IGT-AFI" "Defense Industry" "🎖️" "Token for defense contracting and military services. Pay for defense equipment, military consulting, veteran services, and security training." "Defense Contracts" "Military Equipment" "Veteran Services" "Security Training" "Defense Consulting" "Strategic Services"
create_commercial_page "33-IGT-NIS" "33" "IGT-NIS" "Intelligence Services" "🕵️" "Token for intelligence and investigation services. Pay for private investigations, cybersecurity, threat assessment, and intelligence consulting." "Private Investigation" "Cybersecurity" "Threat Assessment" "Intelligence Reports" "Risk Analysis" "Security Audits"
create_commercial_page "34-IGT-CBP" "34" "IGT-CBP" "Customs & Border" "🛃" "Token for customs and border services. Pay for import duties, customs clearance, border crossing fees, and trade compliance services." "Import Duties" "Customs Clearance" "Border Fees" "Trade Compliance" "Duty-Free Access" "Tariff Services"
create_commercial_page "35-IGT-CRO" "35" "IGT-CRO" "Registry Services" "📋" "Token for civil registry and documentation. Pay for birth certificates, marriage licenses, official documents, and vital records services." "Birth Certificates" "Marriage Licenses" "Official Documents" "Vital Records" "ID Services" "Document Verification"
create_commercial_page "36-IGT-EC" "36" "IGT-EC" "Electoral Services" "🗳️" "Token for electoral and democratic services. Support campaigns, political advertising, election services, and democratic participation platforms." "Campaign Support" "Political Ads" "Election Services" "Voter Registration" "Poll Services" "Democratic Tools"
create_commercial_page "37-IGT-OCG" "37" "IGT-OCG" "Audit & Compliance" "📊" "Token for audit, compliance, and oversight services. Pay for financial audits, compliance reviews, regulatory consulting, and oversight services." "Financial Audits" "Compliance Reviews" "Regulatory Consulting" "Risk Assessment" "Internal Controls" "Oversight Services"
create_commercial_page "38-IGT-OO" "38" "IGT-OO" "Ombudsman Services" "👁️" "Token for ombudsman and citizen services. Access complaint resolution, citizen advocacy, government liaison, and public interest services." "Complaint Resolution" "Citizen Advocacy" "Government Liaison" "Public Interest" "Mediation Services" "Rights Protection"
create_commercial_page "39-IGT-NA" "39" "IGT-NA" "Archives & Records" "📜" "Token for archives, records, and historical services. Access historical documents, archive research, record storage, and preservation services." "Archive Access" "Historical Records" "Research Services" "Document Storage" "Preservation" "Genealogy Services"
create_commercial_page "40-IGT-PSI" "40" "IGT-PSI" "Postal & Delivery" "📮" "Token for postal and delivery services. Pay for mail, packages, courier services, logistics, and last-mile delivery solutions." "Mail Services" "Package Delivery" "Courier Services" "Express Shipping" "PO Box Rental" "Logistics Solutions"

echo "✅ Created 40 Government Commerce Token pages!"
