#!/usr/bin/env python3
"""
create-blockchain-sovereign.py
Genera 10 plataformas blockchain soberanas nuevas para el ecosistema Ierahkwa.
Cada una sigue Pattern B con ~200-220 lineas de HTML.
"""
import os, pathlib

BASE = pathlib.Path(__file__).resolve().parent.parent  # 02-plataformas-html/

PLATFORMS = [
    {
        "dir": "smart-contracts-soberano",
        "title": "Smart Contracts Soberano",
        "subtitle": "Motor de Contratos Inteligentes Post-Cu\u00e1ntico",
        "icon": "\U0001f4dc",
        "accent": "#ffd600",
        "description": "Motor de contratos inteligentes soberano que reemplaza Ethereum/Solidity. Lenguaje MameyLang type-safe, VM soberana con ejecuci\u00f3n determin\u00edstica, gas fees en WAMPUM, verificaci\u00f3n formal integrada, upgradeable contracts, multi-chain deploy. Sin dependencia de Ethereum Foundation.",
        "metrics": [("MameyLang", "Lenguaje"), ("50K TPS", "Throughput"), ("&lt;2s", "Finality"), ("Formal", "Verify"), ("0", "Reentrancy"), ("Upgradeable", "Contracts")],
        "cards": [
            ("\U0001f4dc", "Lenguaje MameyLang Type-Safe", "Lenguaje de programaci\u00f3n propio con tipado est\u00e1tico, pattern matching y verificaci\u00f3n en compilaci\u00f3n. Elimina clases enteras de vulnerabilidades como reentrancy y overflow."),
            ("\u2699\ufe0f", "VM Soberana Determin\u00edstica", "M\u00e1quina virtual optimizada para ejecuci\u00f3n determin\u00edstica de contratos. Metering preciso de gas, sandboxing aislado y reproducibilidad garantizada."),
            ("\U0001f4b0", "Gas Fees en WAMPUM", "Sistema de gas nativo con tarifas en WAMPUM. Estimaci\u00f3n precisa, fee market justo sin MEV y subsidios para contratos de inter\u00e9s p\u00fablico."),
            ("\u2705", "Verificaci\u00f3n Formal Integrada", "Prover SMT integrado que verifica invariantes, precondiciones y postcondiciones. Garant\u00edas matem\u00e1ticas de correctitud antes del deploy."),
            ("\U0001f504", "Upgradeable Proxy Pattern", "Patr\u00f3n de proxy transparente para actualizar l\u00f3gica de contratos sin perder estado. Timelock y gobernanza multi-sig para upgrades seguros."),
            ("\U0001f310", "Multi-Chain Deploy", "Deploy simult\u00e1neo en MameyNode, Ethereum, Solana y 10+ cadenas. Un solo codebase MameyLang, compilaci\u00f3n cruzada autom\u00e1tica."),
            ("\U0001f4e1", "Event System Nativo", "Sistema de eventos tipados con indexaci\u00f3n autom\u00e1tica. Subscripciones WebSocket en tiempo real y logs inmutables en blockchain."),
            ("\U0001f510", "Access Control Granular", "RBAC on-chain con roles jer\u00e1rquicos, permisos por funci\u00f3n y listas de acceso. Compatible con est\u00e1ndares OpenZeppelin mejorados."),
            ("\u23f0", "Time-Lock y Multi-Sig", "Operaciones cr\u00edticas requieren delay temporal y m\u00faltiples firmas. Protecci\u00f3n contra ataques de gobernanza y ejecuci\u00f3n maliciosa."),
            ("\U0001f916", "Auditor\u00eda Automatizada con IA", "Scanner de vulnerabilidades con IA que analiza bytecode y source code. Detecta 200+ patrones de vulnerabilidad conocidos."),
        ],
        "apis": [
            ("POST", "/api/v1/contracts/deploy", "Desplegar contrato compilado. Params: bytecode, abi, constructor_args."),
            ("POST", "/api/v1/contracts/execute", "Ejecutar funci\u00f3n de contrato. Params: address, function, args, gas_limit."),
            ("GET", "/api/v1/contracts/status/{address}", "Estado del contrato: balance, storage, nonce y metadata."),
            ("GET", "/api/v1/contracts/events/{address}", "Eventos emitidos por el contrato con filtros por topic y bloque."),
            ("POST", "/api/v1/contracts/verify", "Verificaci\u00f3n formal del contrato. Params: source, invariants."),
            ("GET", "/api/v1/contracts/abi/{address}", "ABI del contrato desplegado con documentaci\u00f3n de funciones."),
        ],
        "db_stores": ["contracts-deployed", "contract-events", "verification-cache"],
        "arch": [
            ("var(--accent)", "MAMEYLANG COMPILER", "Source Code \u00b7 Type Checking \u00b7 Optimization", "Compilaci\u00f3n type-safe \u00b7 Pattern matching \u00b7 Zero-cost abstractions"),
            ("#00bcd4", "SOVEREIGN VM", "Deterministic Execution \u00b7 Gas Metering", "Sandboxed runtime \u00b7 Memory safety \u00b7 Reproducible"),
            ("#7c4dff", "VERIFICATION ENGINE", "SMT Prover \u00b7 Invariant Check \u00b7 Formal Proof", "Verificaci\u00f3n matem\u00e1tica \u00b7 Pre/post conditions \u00b7 Coverage"),
            ("var(--accent)", "MAMEYNODE BLOCKCHAIN", "Settlement \u00b7 Events \u00b7 State Storage", "Consensus PoS \u00b7 Finality &lt;2s \u00b7 Immutable audit trail"),
        ],
    },
    {
        "dir": "defi-soberano",
        "title": "DeFi Soberano",
        "subtitle": "Finanzas Descentralizadas para Naciones Ind\u00edgenas",
        "icon": "\U0001f3e6",
        "accent": "#ffd600",
        "description": "Ecosistema DeFi soberano que reemplaza Uniswap, Aave y Compound. Lending/borrowing con tasas justas, yield farming soberano, liquidity pools WAMPUM, stablecoins respaldadas por recursos naturales, seguros descentralizados, todo gobernado por las 19 naciones.",
        "metrics": [("$500M", "TVL"), ("8", "Protocolos"), ("Justo", "APY"), ("0", "Rug Pulls"), ("19", "Naciones"), ("Audited", "Seguridad")],
        "cards": [
            ("\U0001f4b0", "Lending y Borrowing Soberano", "Protocolo de pr\u00e9stamos descentralizado con tasas justas determinadas por gobernanza. Colateralizaci\u00f3n flexible con activos nativos y cross-chain."),
            ("\U0001f33e", "Yield Farming Comunitario", "Farming de rendimiento donde las recompensas benefician a comunidades enteras. Pools verificados, sin rugs, con auditor\u00eda transparente."),
            ("\U0001f4a7", "Liquidity Pools WAMPUM", "Pools de liquidez nativas para pares WAMPUM con curvas de precio optimizadas. Impermanent loss minimizado con algoritmos soberanos."),
            ("\U0001f3d4\ufe0f", "Stablecoins de Recursos Naturales", "Stablecoins respaldadas por recursos naturales verificados: tierra, agua, bosques, minerales. Valor real, no sint\u00e9tico."),
            ("\U0001f6e1\ufe0f", "Seguros Descentralizados", "Protocolos de seguros peer-to-peer para smart contracts, bridges y liquidaciones. Claims autom\u00e1ticos sin intermediarios."),
            ("\u26a1", "Flash Loans Controlados", "Pr\u00e9stamos flash con l\u00edmites de seguridad y uso \u00e9tico. Prevenci\u00f3n de ataques de manipulaci\u00f3n y arbitraje predatorio."),
            ("\U0001f3e6", "Vaults de Rendimiento", "Estrategias automatizadas de rendimiento con auto-compounding. Optimizaci\u00f3n de yields entre protocolos soberanos."),
            ("\U0001f3db\ufe0f", "Gobernanza por Naciones", "Cada naci\u00f3n tiene voz y voto en par\u00e1metros DeFi: tasas, colateral, l\u00edmites. Democracia financiera descentralizada."),
            ("\u2696\ufe0f", "Liquidaci\u00f3n Justa sin MEV", "Motor de liquidaci\u00f3n que protege a usuarios de MEV y front-running. Subastas holandesas justas con precio m\u00ednimo garantizado."),
            ("\U0001f4ca", "Analytics DeFi Transparente", "Dashboard p\u00fablico con m\u00e9tricas de TVL, yields, health factors y riesgos. Transparencia total para todos los participantes."),
        ],
        "apis": [
            ("POST", "/api/v1/defi/lend", "Depositar activos en pool de lending. Params: asset, amount, duration."),
            ("POST", "/api/v1/defi/borrow", "Solicitar pr\u00e9stamo. Params: collateral, borrow_asset, amount, ltv."),
            ("GET", "/api/v1/defi/pools", "Lista de pools activos con TVL, APY y m\u00e9tricas de riesgo."),
            ("GET", "/api/v1/defi/yields/{protocol}", "Rendimientos hist\u00f3ricos y proyectados por protocolo."),
            ("POST", "/api/v1/defi/swap", "Intercambio de tokens con routing \u00f3ptimo multi-pool."),
            ("GET", "/api/v1/defi/tvl", "Total Value Locked global y desglose por protocolo y naci\u00f3n."),
        ],
        "db_stores": ["defi-positions", "pool-liquidity", "yield-history"],
        "arch": [
            ("var(--accent)", "USUARIO / WALLET", "Dep\u00f3sito \u00b7 Pr\u00e9stamo \u00b7 Farming \u00b7 Swap", "Multi-wallet \u00b7 Biom\u00e9trico \u00b7 Firma soberana"),
            ("#00bcd4", "SMART CONTRACTS DEFI", "Lending \u00b7 Borrowing \u00b7 AMM \u00b7 Vaults", "Audited \u00b7 Formal verification \u00b7 Upgradeable"),
            ("#7c4dff", "LIQUIDITY POOLS", "WAMPUM Pairs \u00b7 Stablecoins \u00b7 LP Tokens", "Curvas optimizadas \u00b7 IL protection \u00b7 Fee tiers"),
            ("var(--accent)", "MAMEYNODE SETTLEMENT", "Consensus \u00b7 Finality \u00b7 State \u00b7 Events", "PoS soberano \u00b7 &lt;2s finality \u00b7 Immutable"),
        ],
    },
    {
        "dir": "dex-soberano",
        "title": "DEX Soberano",
        "subtitle": "Exchange Descentralizado sin Intermediarios",
        "icon": "\U0001f504",
        "accent": "#ffd600",
        "description": "Exchange descentralizado que reemplaza Uniswap y PancakeSwap. AMM soberano con curvas de precio optimizadas, order book on-chain, cross-chain swaps at\u00f3micos, zero front-running con encrypted mempool, liquidity mining WAMPUM, trading sin KYC respetando soberan\u00eda.",
        "metrics": [("$100M", "Vol/d\u00eda"), ("200+", "Pares"), ("&lt;1s", "Swaps"), ("0", "Front-run"), ("AMM+OB", "H\u00edbrido"), ("Cross-chain", "Swaps")],
        "cards": [
            ("\U0001f4c8", "AMM con Curvas Optimizadas", "Automated Market Maker con curvas de precio personalizadas por par. Concentrated liquidity para m\u00e1xima eficiencia de capital."),
            ("\U0001f4cb", "Order Book On-Chain H\u00edbrido", "Libro de \u00f3rdenes on-chain combinado con AMM para mejor descubrimiento de precios. Limit orders sin custodia."),
            ("\U0001f517", "Cross-Chain Swaps At\u00f3micos", "Intercambios at\u00f3micos entre MameyNode, Ethereum, Bitcoin y Solana. Sin bridges intermediarios, sin riesgo de contraparte."),
            ("\U0001f512", "Encrypted Mempool Anti-MEV", "Mempool cifrado que previene front-running y sandwich attacks. Transacciones reveladas solo al incluirse en bloque."),
            ("\u26cf\ufe0f", "Liquidity Mining WAMPUM", "Incentivos de miner\u00eda de liquidez en WAMPUM para proveedores. Emisi\u00f3n decreciente con halving programado."),
            ("\U0001f3af", "Limit Orders Descentralizadas", "\u00d3rdenes l\u00edmite on-chain con ejecuci\u00f3n garantizada al precio especificado. Sin keepers centralizados."),
            ("\U0001f4a7", "Concentrated Liquidity", "Provisi\u00f3n de liquidez en rangos de precio espec\u00edficos para maximizar rendimiento. Similar a Uniswap v3 pero soberano."),
            ("\u2699\ufe0f", "Fee Tiers Din\u00e1micos", "Comisiones din\u00e1micas que se ajustan por volatilidad del par. Tiers de 0.01%, 0.05%, 0.3% y 1% configurables."),
            ("\U0001f4ca", "Portfolio Tracker Nativo", "Seguimiento de portfolio integrado con P&amp;L, impermanent loss y rendimiento hist\u00f3rico. Sin trackers externos."),
            ("\U0001f50d", "Trading Analytics Zero-Track", "Analytics avanzados sin tracking personal. M\u00e9tricas agregadas del mercado con privacidad total del trader."),
        ],
        "apis": [
            ("POST", "/api/v1/dex/swap", "Ejecutar swap entre tokens. Params: token_in, token_out, amount, slippage."),
            ("GET", "/api/v1/dex/pairs", "Lista de pares disponibles con precio, liquidez y volumen 24h."),
            ("GET", "/api/v1/dex/price/{pair}", "Precio actual del par con spread, depth y precio medio."),
            ("POST", "/api/v1/dex/order", "Crear limit order. Params: pair, side, price, amount, expiry."),
            ("GET", "/api/v1/dex/liquidity/{pool}", "Liquidez del pool con distribuci\u00f3n por rango de precio."),
            ("GET", "/api/v1/dex/history/{pair}", "Historial de trades del par con OHLCV y volumen."),
        ],
        "db_stores": ["dex-orders", "pair-prices", "liquidity-positions"],
        "arch": [
            ("var(--accent)", "TRADER", "Swap \u00b7 Limit Order \u00b7 LP Provision", "Wallet connect \u00b7 Firma soberana \u00b7 Zero-track"),
            ("#00bcd4", "ENCRYPTED MEMPOOL", "Transaction Privacy \u00b7 Anti-MEV \u00b7 Commit-Reveal", "Cifrado post-cu\u00e1ntico \u00b7 Reveal on inclusion"),
            ("#7c4dff", "AMM / ORDERBOOK ENGINE", "Curvas \u00b7 Matching \u00b7 Concentrated Liquidity", "Routing \u00f3ptimo \u00b7 Fee tiers \u00b7 Price discovery"),
            ("var(--accent)", "SETTLEMENT LAYER", "MameyNode \u00b7 Finality \u00b7 State Update", "Atomic settlement \u00b7 Cross-chain \u00b7 Immutable"),
        ],
    },
    {
        "dir": "dao-soberano",
        "title": "DAO Soberano",
        "subtitle": "Organizaci\u00f3n Aut\u00f3noma Descentralizada para 19 Naciones",
        "icon": "\U0001f3db\ufe0f",
        "accent": "#1565c0",
        "description": "Framework DAO soberano que reemplaza Aragon y Snapshot. Gobernanza on-chain para 19 naciones con votaci\u00f3n cuadr\u00e1tica, propuestas con quorum adaptativo, tesorer\u00eda multi-sig, delegaci\u00f3n de votos, sub-DAOs por naci\u00f3n, transparencia total con blockchain auditable.",
        "metrics": [("19", "Naciones"), ("72M", "Votantes"), ("Cuadr\u00e1tica", "Votaci\u00f3n"), ("Adaptativo", "Quorum"), ("Multi-sig", "Tesorer\u00eda"), ("100%", "Transparente")],
        "cards": [
            ("\U0001f5f3\ufe0f", "Votaci\u00f3n Cuadr\u00e1tica On-Chain", "Sistema de votaci\u00f3n cuadr\u00e1tica que previene plutocracias. El costo de votos adicionales crece cuadr\u00e1ticamente, dando voz a las minor\u00edas."),
            ("\U0001f4cb", "Propuestas con Quorum Adaptativo", "Quorum que se ajusta din\u00e1micamente seg\u00fan participaci\u00f3n hist\u00f3rica. Previene bloqueos por baja participaci\u00f3n sin sacrificar legitimidad."),
            ("\U0001f4b0", "Tesorer\u00eda Multi-Sig Naciones", "Fondos comunitarios custodiados por multi-sig de representantes nacionales. Cada naci\u00f3n tiene una clave, requiere mayor\u00eda para gastos."),
            ("\U0001f504", "Delegaci\u00f3n de Votos L\u00edquida", "Delega tu poder de voto a representantes de confianza con revocaci\u00f3n instant\u00e1nea. Delegaci\u00f3n transitiva y por categor\u00eda."),
            ("\U0001f3d8\ufe0f", "Sub-DAOs por Naci\u00f3n", "Cada naci\u00f3n opera su propia sub-DAO con autonom\u00eda local. Gobernanza federal: decisiones locales locales, globales globales."),
            ("\u23f0", "Timelock de Ejecuci\u00f3n", "Todas las propuestas aprobadas pasan por timelock configurable. Tiempo de gracia para que la comunidad revise y objete."),
            ("\U0001f4f8", "Snapshot Off-Chain + On-Chain", "Votaci\u00f3n off-chain para se\u00f1alizaci\u00f3n r\u00e1pida y gratuita. On-chain para decisiones vinculantes con ejecuci\u00f3n autom\u00e1tica."),
            ("\u2b50", "Reputation-Weighted Voting", "Peso de voto ajustado por reputaci\u00f3n construida con participaci\u00f3n, propuestas exitosas y contribuciones verificadas."),
            ("\U0001f4ca", "Treasury Diversification", "Estrategia autom\u00e1tica de diversificaci\u00f3n de tesorer\u00eda entre WAMPUM, stablecoins y activos productivos."),
            ("\U0001f50d", "Transparency Dashboard", "Dashboard p\u00fablico con todas las propuestas, votos, gastos y decisiones. Auditor\u00eda ciudadana en tiempo real."),
        ],
        "apis": [
            ("POST", "/api/v1/dao/propose", "Crear propuesta. Params: title, description, actions, voting_period."),
            ("POST", "/api/v1/dao/vote", "Votar en propuesta. Params: proposal_id, support, amount."),
            ("GET", "/api/v1/dao/proposals", "Lista de propuestas con estado, votos y timeline."),
            ("GET", "/api/v1/dao/treasury", "Estado de tesorer\u00eda con balance, inflows, outflows y allocations."),
            ("GET", "/api/v1/dao/members/{nation}", "Miembros registrados por naci\u00f3n con poder de voto y delegaciones."),
            ("POST", "/api/v1/dao/delegate", "Delegar votos. Params: delegate_to, amount, categories."),
        ],
        "db_stores": ["dao-proposals", "vote-records", "treasury-state"],
        "arch": [
            ("var(--accent)", "CIUDADANO / NACI\u00d3N", "Propuesta \u00b7 Voto \u00b7 Delegaci\u00f3n", "Identidad soberana \u00b7 19 naciones \u00b7 72M personas"),
            ("#00bcd4", "PROPUESTA ENGINE", "Draft \u00b7 Discusi\u00f3n \u00b7 Snapshot \u00b7 Formal", "Quorum adaptativo \u00b7 Timelock \u00b7 Categories"),
            ("#7c4dff", "VOTACI\u00d3N CUADR\u00c1TICA", "Quadratic \u00b7 Reputation \u00b7 Delegation", "Anti-plutocracy \u00b7 Sybil-resistant \u00b7 Verifiable"),
            ("var(--accent)", "EJECUCI\u00d3N TIMELOCK", "Multi-sig \u00b7 Delay \u00b7 Execution \u00b7 Audit", "On-chain execution \u00b7 Immutable record \u00b7 Transparent"),
        ],
    },
    {
        "dir": "bridge-soberano",
        "title": "Bridge Soberano",
        "subtitle": "Puente de Interoperabilidad Multi-Chain",
        "icon": "\U0001f309",
        "accent": "#00bcd4",
        "description": "Bridge de interoperabilidad que reemplaza Polkadot/Cosmos IBC. Conecta MameyNode con Ethereum, Bitcoin, Solana y 15+ cadenas. Transferencias cross-chain seguras con MPC y validadores soberanos, relay chain nativo, mensajer\u00eda inter-chain, compatible con IBC y XCM.",
        "metrics": [("15+", "Chains"), ("MPC", "Seguro"), ("&lt;30s", "Bridge"), ("0", "Hacks"), ("Relay", "Nativo"), ("$1B", "Transferido")],
        "cards": [
            ("\U0001f517", "Relay Chain Soberano", "Cadena relay propia que coordina comunicaci\u00f3n entre blockchains. Consenso BFT con validadores de las 19 naciones."),
            ("\U0001f510", "MPC Validators Distribuidos", "Validadores con Multi-Party Computation distribuidos globalmente. Sin punto \u00fanico de fallo, sin custodia centralizada."),
            ("\u27e0", "Bridge Ethereum Bidireccional", "Puente bidireccional a Ethereum con pruebas SPV y validaci\u00f3n on-chain. Transferencias de ETH, ERC-20 y NFTs."),
            ("\u20bf", "Bridge Bitcoin via HTLCs", "Conexi\u00f3n con Bitcoin mediante Hash Time-Locked Contracts. Swaps at\u00f3micos BTC-WAMPUM sin intermediarios."),
            ("\U0001f310", "Compatible IBC Cosmos", "Implementaci\u00f3n nativa de Inter-Blockchain Communication. Interoperabilidad con todo el ecosistema Cosmos."),
            ("\u26a1", "Compatible XCM Polkadot", "Soporte para Cross-Consensus Messaging de Polkadot. Mensajes y transferencias con parachains."),
            ("\U0001f4e8", "Mensajer\u00eda Inter-Chain", "Protocolo de mensajer\u00eda gen\u00e9rica entre cadenas. Llamadas cross-chain a smart contracts remotos."),
            ("\U0001f381", "Wrapped Assets Soberanos", "Tokens wrapped con colateral verificable on-chain. Transparencia total del respaldo de activos bridgeados."),
            ("\U0001f4ca", "Bridge Monitor Dashboard", "Monitoreo en tiempo real de todas las transferencias cross-chain. Alertas de anomal\u00edas y m\u00e9tricas de salud."),
            ("\U0001f6a8", "Emergency Pause Multi-Sig", "Sistema de pausa de emergencia controlado por multi-sig de 19 naciones. Protecci\u00f3n instant\u00e1nea ante exploits."),
        ],
        "apis": [
            ("POST", "/api/v1/bridge/transfer", "Iniciar transferencia cross-chain. Params: from_chain, to_chain, asset, amount."),
            ("GET", "/api/v1/bridge/chains", "Lista de cadenas soportadas con estado de conexi\u00f3n y fees."),
            ("GET", "/api/v1/bridge/status/{tx_id}", "Estado de transferencia: pending, validating, confirmed, completed."),
            ("GET", "/api/v1/bridge/validators", "Validadores activos con stake, uptime y rendimiento."),
            ("POST", "/api/v1/bridge/wrap", "Crear wrapped asset. Params: chain, asset, amount, destination."),
            ("GET", "/api/v1/bridge/history", "Historial de transferencias con filtros por chain, asset y fecha."),
        ],
        "db_stores": ["bridge-transfers", "validator-state", "wrapped-assets"],
        "arch": [
            ("var(--accent)", "CHAIN A (SOURCE)", "Lock Assets \u00b7 Emit Proof \u00b7 Monitor", "Ethereum \u00b7 Bitcoin \u00b7 Solana \u00b7 Cosmos \u00b7 15+ chains"),
            ("#00bcd4", "MPC VALIDATORS", "Threshold Signatures \u00b7 Attestation \u00b7 Consensus", "Distribuido \u00b7 19 naciones \u00b7 Sin custodia central"),
            ("#7c4dff", "RELAY CHAIN SOBERANO", "Message Routing \u00b7 State Verification \u00b7 Finality", "BFT consensus \u00b7 IBC compatible \u00b7 XCM compatible"),
            ("var(--accent)", "CHAIN B (DESTINATION)", "Mint/Unlock \u00b7 Verify Proof \u00b7 Settle", "MameyNode \u00b7 Cross-chain execution \u00b7 Immutable"),
        ],
    },
    {
        "dir": "oracle-soberano",
        "title": "Oracle Soberano",
        "subtitle": "Red de Or\u00e1culos Descentralizados para Datos Reales",
        "icon": "\U0001f52e",
        "accent": "#7c4dff",
        "description": "Red de or\u00e1culos descentralizados que reemplaza Chainlink y Band Protocol. Feeds de precios WAMPUM, datos clim\u00e1ticos para agricultura, datos de salud p\u00fablica, randomness verificable (VRF), computaci\u00f3n off-chain, nodos operados por las 19 naciones con staking soberano.",
        "metrics": [("847", "Nodos"), ("1000+", "Feeds"), ("&lt;1s", "Update"), ("VRF", "Nativo"), ("19", "Naciones"), ("99.99%", "Uptime")],
        "cards": [
            ("\U0001f4b2", "Price Feeds WAMPUM Multi-Asset", "Feeds de precios en tiempo real para WAMPUM y 200+ activos. Aggregation multi-source con detecci\u00f3n de outliers."),
            ("\U0001f326\ufe0f", "Datos Clim\u00e1ticos para Agricultura", "Datos meteorol\u00f3gicos verificados para smart contracts agr\u00edcolas. Temperatura, lluvia, humedad y predicciones para 574 territorios."),
            ("\U0001f3e5", "Datos de Salud P\u00fablica", "Indicadores de salud p\u00fablica anonimizados para protocolos de seguros y planificaci\u00f3n. Epidemiolog\u00eda, vacunaci\u00f3n, nutrici\u00f3n."),
            ("\U0001f3b2", "VRF Randomness Verificable", "Generaci\u00f3n de n\u00fameros aleatorios verificables on-chain. Para loter\u00edas, gaming, selecci\u00f3n de jurados y asignaci\u00f3n justa."),
            ("\U0001f5a5\ufe0f", "Computaci\u00f3n Off-Chain Segura", "Ejecuci\u00f3n de computaci\u00f3n pesada off-chain con pruebas verificables on-chain. Machine learning, simulaciones, analytics."),
            ("\U0001f30d", "Nodos Soberanos por Naci\u00f3n", "Cada naci\u00f3n opera nodos oracle con stake propio. Descentralizaci\u00f3n real, no corporativa."),
            ("\U0001f4ca", "Aggregation Multi-Source", "Agregaci\u00f3n de m\u00faltiples fuentes de datos con mediana ponderada. Resistente a manipulaci\u00f3n y outliers."),
            ("\U0001f493", "Heartbeat Monitoring", "Monitoreo de latido de nodos con alertas autom\u00e1ticas. Detecci\u00f3n de nodos ca\u00eddos y failover instant\u00e1neo."),
            ("\u2b50", "Data Quality Scoring", "Puntuaci\u00f3n de calidad de datos por fuente. Incentivos para datos precisos, penalizaciones para inexactos."),
            ("\u2696\ufe0f", "Dispute Resolution DAO", "Resoluci\u00f3n de disputas sobre datos mediante votaci\u00f3n DAO. Arbitraje descentralizado para feeds controversiales."),
        ],
        "apis": [
            ("GET", "/api/v1/oracle/prices/{asset}", "Precio actual del activo con timestamp, sources y confidence."),
            ("GET", "/api/v1/oracle/weather/{territory}", "Datos clim\u00e1ticos del territorio con hist\u00f3rico y predicci\u00f3n."),
            ("GET", "/api/v1/oracle/health/{region}", "Indicadores de salud p\u00fablica de la regi\u00f3n."),
            ("POST", "/api/v1/oracle/vrf", "Solicitar n\u00famero aleatorio verificable. Params: seed, callback."),
            ("GET", "/api/v1/oracle/feeds", "Lista de todos los feeds disponibles con frecuencia y quality score."),
            ("GET", "/api/v1/oracle/nodes", "Nodos oracle activos con stake, uptime y performance score."),
        ],
        "db_stores": ["oracle-feeds", "vrf-requests", "node-reputation"],
        "arch": [
            ("var(--accent)", "DATA SOURCES", "APIs \u00b7 Sensores \u00b7 Sat\u00e9lites \u00b7 Gobierno", "Multi-source \u00b7 Verified \u00b7 Real-time \u00b7 Historical"),
            ("#00bcd4", "ORACLE NODES (19 NACIONES)", "Fetch \u00b7 Validate \u00b7 Attest \u00b7 Stake", "847 nodos \u00b7 Distribuido global \u00b7 Slashing justo"),
            ("#7c4dff", "AGGREGATION ENGINE", "Median \u00b7 Outlier Detection \u00b7 Quality Score", "Resistente a manipulaci\u00f3n \u00b7 Weighted \u00b7 Verifiable"),
            ("var(--accent)", "SMART CONTRACTS CONSUMERS", "Price Feeds \u00b7 VRF \u00b7 Weather \u00b7 Health", "On-chain delivery \u00b7 Callback \u00b7 Event-driven"),
        ],
    },
    {
        "dir": "storage-descentralizado-soberano",
        "title": "Storage Descentralizado Soberano",
        "subtitle": "Almacenamiento Distribuido Soberano tipo IPFS",
        "icon": "\U0001f4be",
        "accent": "#00e676",
        "description": "Red de almacenamiento descentralizado que reemplaza IPFS, Filecoin y Arweave. Content-addressable storage con CIDs soberanos, pinning en 847 nodos, encryption at rest post-cu\u00e1ntica, incentivos de storage con WAMPUM, archivado permanente de cultura ind\u00edgena.",
        "metrics": [("847", "Nodos"), ("PB+", "Storage"), ("CID", "Soberano"), ("E2E", "Cifrado"), ("Permanente", "Archivado"), ("WAMPUM", "Incentivado")],
        "cards": [
            ("\U0001f4e6", "Content-Addressable Storage", "Almacenamiento direccionado por contenido con hashes criptogr\u00e1ficos. Deduplicaci\u00f3n autom\u00e1tica e integridad verificable."),
            ("\U0001f4cc", "Pinning Distribuido 847 Nodos", "Pinning redundante en nodos soberanos de 19 naciones. Garant\u00eda de disponibilidad sin dependencia de pinning services."),
            ("\U0001f510", "Encryption At-Rest Post-Cu\u00e1ntica", "Todos los archivos cifrados con algoritmos post-cu\u00e1nticos Kyber-768 en reposo. Solo el propietario puede descifrar."),
            ("\U0001f4b0", "Incentivos Storage WAMPUM", "Nodos recompensados en WAMPUM por almacenar y servir datos. Pruebas de almacenamiento verificables on-chain."),
            ("\U0001f3db\ufe0f", "Archivado Cultural Permanente", "Almacenamiento permanente de lenguas, tradiciones, historia y arte ind\u00edgena. Resiliencia generacional garantizada."),
            ("\U0001f9f9", "Deduplication Inteligente", "Deduplicaci\u00f3n a nivel de chunk con \u00e1rboles Merkle. Ahorro de espacio sin perder redundancia de seguridad."),
            ("\U0001f310", "CDN Descentralizado", "Red de distribuci\u00f3n de contenido integrada. Archivos servidos desde el nodo m\u00e1s cercano geogr\u00e1ficamente."),
            ("\u267b\ufe0f", "Garbage Collection Configurable", "Recolecci\u00f3n de basura configurable por pol\u00edticas. Datos no pinneados liberados seg\u00fan reglas de la red."),
            ("\U0001f517", "IPFS Gateway Compatible", "Gateway HTTP compatible con IPFS para acceso desde navegadores est\u00e1ndar. URLs legibles y compartibles."),
            ("\U0001f4f9", "Streaming de Archivos Grandes", "Soporte nativo para streaming de video, audio y datasets grandes. Chunking adaptativo y pre-fetching inteligente."),
        ],
        "apis": [
            ("POST", "/api/v1/storage/upload", "Subir archivo a la red. Params: file, encrypt, pin_nations, ttl."),
            ("GET", "/api/v1/storage/download/{cid}", "Descargar archivo por CID con verificaci\u00f3n de integridad."),
            ("GET", "/api/v1/storage/pin/{cid}", "Estado de pinning del CID: nodos, regiones, redundancia."),
            ("GET", "/api/v1/storage/status", "Estado general de la red: capacidad, uso, nodos activos."),
            ("POST", "/api/v1/storage/archive", "Archivar permanentemente. Params: cid, metadata, category."),
            ("GET", "/api/v1/storage/nodes", "Lista de nodos de almacenamiento con capacidad y ubicaci\u00f3n."),
        ],
        "db_stores": ["storage-metadata", "pin-registry", "archive-manifest"],
        "arch": [
            ("var(--accent)", "ARCHIVO / UPLOAD", "Chunking \u00b7 Encrypt \u00b7 Hash \u00b7 Metadata", "Content-addressable \u00b7 Kyber-768 \u00b7 CID soberano"),
            ("#00bcd4", "DHT ROUTING", "Peer Discovery \u00b7 Content Routing \u00b7 Kademlia", "Distribuido \u00b7 Tolerant \u00b7 Low-latency"),
            ("#7c4dff", "STORAGE NODES", "Store \u00b7 Prove \u00b7 Replicate \u00b7 Serve", "847 nodos \u00b7 19 naciones \u00b7 Proof of Storage"),
            ("var(--accent)", "INCENTIVE LAYER", "WAMPUM Rewards \u00b7 Slashing \u00b7 Proof Verification", "On-chain \u00b7 Fair \u00b7 Transparent \u00b7 Sustainable"),
        ],
    },
    {
        "dir": "layer2-soberano",
        "title": "Layer 2 Soberano",
        "subtitle": "Soluci\u00f3n de Escalabilidad para MameyNode Blockchain",
        "icon": "\u26a1",
        "accent": "#ffd600",
        "description": "Soluci\u00f3n Layer 2 que reemplaza Optimism, Arbitrum y zkSync. Rollups soberanos con zero-knowledge proofs, 100K+ TPS, finality en 1 segundo, state channels para micropagos, plasma chains para datos masivos, sequencer descentralizado operado por las naciones.",
        "metrics": [("100K+", "TPS"), ("1s", "Finality"), ("ZK", "Proofs"), ("Channels", "State"), ("Desc.", "Sequencer"), ("99.999%", "Uptime")],
        "cards": [
            ("\U0001f512", "ZK-Rollups Soberanos", "Rollups con zero-knowledge proofs para m\u00e1xima seguridad y privacidad. Validez criptogr\u00e1fica sin revelar datos de transacciones."),
            ("\U0001f504", "Optimistic Rollups H\u00edbridos", "Rollups optimistic para casos donde la velocidad prima sobre privacidad. Challenge period configurable por las naciones."),
            ("\U0001f4b8", "State Channels para Micropagos", "Canales de pago bidireccionales para micropagos instant\u00e1neos. Liquidaci\u00f3n on-chain solo al cerrar canal."),
            ("\U0001f4ca", "Plasma Chains para Datos", "Chains plasma especializadas en datos masivos: IoT, sensores, logging. Alta capacidad con seguridad heredada de L1."),
            ("\U0001f3db\ufe0f", "Sequencer Descentralizado", "Sequencer operado rotativamente por las 19 naciones. Sin centralizaci\u00f3n, sin censura, con inclusi\u00f3n forzada."),
            ("\u26a1", "Proof Aggregation Eficiente", "Agregaci\u00f3n de m\u00faltiples proofs en una sola verificaci\u00f3n L1. Reduce costos de gas y aumenta throughput."),
            ("\U0001f4be", "Data Availability Layer", "Capa de disponibilidad de datos soberana. Garant\u00eda de acceso a datos sin depender de servicios externos."),
            ("\U0001f510", "Forced Withdrawals Seguras", "Retiros forzados que bypasean el sequencer si es necesario. Fondos nunca retenidos, soberan\u00eda del usuario garantizada."),
            ("\U0001f310", "Cross-L2 Communication", "Comunicaci\u00f3n entre diferentes instancias L2. Mensajes y transferencias entre rollups sin pasar por L1."),
            ("\U0001f4c8", "L2 Explorer Dashboard", "Explorador de bloques especializado para L2 con m\u00e9tricas de batch, proofs y gas savings visualizados."),
        ],
        "apis": [
            ("POST", "/api/v1/l2/submit", "Enviar transacci\u00f3n L2. Params: to, value, data, l2_type."),
            ("GET", "/api/v1/l2/status/{batch_id}", "Estado del batch: sequenced, proved, finalized."),
            ("GET", "/api/v1/l2/proofs/{batch_id}", "Proofs del batch con metadata de verificaci\u00f3n."),
            ("POST", "/api/v1/l2/channel", "Abrir state channel. Params: counterparty, deposit, ttl."),
            ("GET", "/api/v1/l2/batches", "Lista de batches recientes con transacciones y estado."),
            ("GET", "/api/v1/l2/bridge", "Estado del bridge L1-L2 con pending deposits y withdrawals."),
        ],
        "db_stores": ["l2-transactions", "proof-cache", "channel-state"],
        "arch": [
            ("var(--accent)", "TRANSACCIONES L2", "User Txs \u00b7 Micropagos \u00b7 Data \u00b7 DApp Calls", "100K+ TPS \u00b7 Instant \u00b7 Low cost \u00b7 Privacy"),
            ("#00bcd4", "SEQUENCER DESCENTRALIZADO", "Ordering \u00b7 Batching \u00b7 Compression \u00b7 MEV Protection", "Rotativo 19 naciones \u00b7 Forced inclusion \u00b7 Fair"),
            ("#7c4dff", "ZK PROVER", "Proof Generation \u00b7 Aggregation \u00b7 Verification", "Zero-knowledge \u00b7 Recursive proofs \u00b7 GPU accelerated"),
            ("var(--accent)", "L1 MAMEYNODE SETTLEMENT", "Proof Verification \u00b7 State Root \u00b7 DA \u00b7 Finality", "Security inherited \u00b7 Immutable \u00b7 &lt;2s finality"),
        ],
    },
    {
        "dir": "staking-soberano",
        "title": "Staking Soberano",
        "subtitle": "Validaci\u00f3n y Staking para la Red MameyNode",
        "icon": "\u26cf\ufe0f",
        "accent": "#ffd600",
        "description": "Plataforma de staking y validaci\u00f3n que reemplaza Lido y Rocket Pool. Staking l\u00edquido de WAMPUM, validadores soberanos por naci\u00f3n, delegaci\u00f3n con slashing justo, rewards distribuidos equitativamente, staking pool comunitario, dashboard de rendimiento en tiempo real.",
        "metrics": [("847", "Validadores"), ("19", "Naciones"), ("Justo", "APR"), ("Justo", "Slashing"), ("L\u00edquido", "Staking"), ("Real-time", "Dashboard")],
        "cards": [
            ("\U0001f4a7", "Staking L\u00edquido WAMPUM", "Stake WAMPUM y recibe stWAMPUM l\u00edquido para usar en DeFi mientras ganas rewards. Sin lock-up, sin p\u00e9rdida de oportunidad."),
            ("\U0001f30d", "Validadores por Naci\u00f3n", "Cada naci\u00f3n opera validadores propios con stake soberano. Distribuci\u00f3n geogr\u00e1fica real, no concentrada en data centers."),
            ("\u2696\ufe0f", "Delegaci\u00f3n con Slashing Justo", "Delega a validadores de confianza con slashing proporcional y justo. Penalizaciones graduales, no draconianas."),
            ("\U0001f381", "Rewards Equitativos", "Distribuci\u00f3n de recompensas proporcional al stake con bonus comunitario. Sin ventaja para ballenas, fair play."),
            ("\U0001f465", "Staking Pool Comunitario", "Pools de staking para peque\u00f1os holders que no alcanzan el m\u00ednimo de validador. Staking accesible para todos."),
            ("\U0001f4c8", "Auto-Compounding", "Reinversi\u00f3n autom\u00e1tica de rewards en stake. Crecimiento compuesto sin intervenci\u00f3n manual del staker."),
            ("\U0001f4ca", "Validator Dashboard Real-Time", "Dashboard en tiempo real con m\u00e9tricas de uptime, attestations, proposales y slashing history de cada validador."),
            ("\U0001f6b6", "Withdrawal Queue Justa", "Cola de retiro FIFO justa sin priorizaci\u00f3n por tama\u00f1o. Tiempos de espera transparentes y predecibles."),
            ("\U0001f6e1\ufe0f", "MEV Protection", "Validadores protegidos contra MEV extraction. Block building justo con PBS (Proposer-Builder Separation)."),
            ("\U0001f5f3\ufe0f", "Governance por Stakers", "Stakers votan en par\u00e1metros de la red: emisi\u00f3n, slashing, fees. Gobernanza basada en skin-in-the-game."),
        ],
        "apis": [
            ("POST", "/api/v1/staking/stake", "Depositar WAMPUM en staking. Params: amount, validator, auto_compound."),
            ("POST", "/api/v1/staking/unstake", "Iniciar retiro de stake. Params: amount, withdrawal_address."),
            ("GET", "/api/v1/staking/validators", "Lista de validadores con stake, APR, uptime y performance."),
            ("GET", "/api/v1/staking/rewards/{address}", "Rewards acumulados y distribuidos del staker."),
            ("GET", "/api/v1/staking/apr", "APR actual de la red con proyecci\u00f3n e hist\u00f3rico."),
            ("POST", "/api/v1/staking/delegate", "Delegar stake a validador. Params: validator, amount."),
        ],
        "db_stores": ["staking-positions", "validator-metrics", "reward-history"],
        "arch": [
            ("var(--accent)", "STAKER / DELEGATOR", "Deposit \u00b7 Delegate \u00b7 Compound \u00b7 Withdraw", "Liquid staking \u00b7 stWAMPUM \u00b7 Multi-validator"),
            ("#00bcd4", "DELEGATION LAYER", "Pool Management \u00b7 Validator Selection \u00b7 Queue", "Fair FIFO \u00b7 Community pools \u00b7 Auto-compound"),
            ("#7c4dff", "VALIDATOR NODES (19 NACIONES)", "Block Production \u00b7 Attestation \u00b7 MEV Protection", "847 nodos \u00b7 PBS \u00b7 Geographic distribution"),
            ("var(--accent)", "CONSENSUS + REWARDS", "PoS \u00b7 Finality \u00b7 Emission \u00b7 Slashing", "Fair rewards \u00b7 Proportional \u00b7 Transparent"),
        ],
    },
    {
        "dir": "dapps-soberano",
        "title": "DApps Soberano",
        "subtitle": "Framework para Aplicaciones Descentralizadas Soberanas",
        "icon": "\U0001f4f1",
        "accent": "#00e676",
        "description": "Framework para DApps que reemplaza Truffle, Hardhat y Foundry. SDK completo para desarrollar aplicaciones descentralizadas sobre MameyNode, templates para DeFi/NFT/DAO/Gaming, testing integrado, deploy multi-chain, frontend React/Vue con wallet connect soberano.",
        "metrics": [("50+", "Templates"), ("Multi-chain", "Deploy"), ("Nativo", "Testing"), ("Hot Reload", "Dev"), ("Wallet", "Connect"), ("SDK", "Completo")],
        "cards": [
            ("\U0001f9f0", "SDK Completo MameyNode", "Kit de desarrollo con librer\u00edas para interactuar con MameyNode blockchain. TypeScript, Rust y Python con documentaci\u00f3n completa."),
            ("\U0001f4cb", "Templates DeFi/NFT/DAO/Gaming", "50+ templates productivos para DeFi, NFT marketplaces, DAOs y gaming. C\u00f3digo auditado y listo para producci\u00f3n."),
            ("\U0001f9ea", "Testing Framework Integrado", "Framework de testing con unit tests, integration tests y fuzzing. Simulaci\u00f3n de blockchain local con time travel."),
            ("\U0001f680", "Deploy Multi-Chain Autom\u00e1tico", "Deploy a MameyNode, Ethereum, Solana y 10+ chains con un solo comando. Verificaci\u00f3n autom\u00e1tica de contratos."),
            ("\u269b\ufe0f", "Frontend React/Vue Native", "Componentes React y Vue pre-built para DApps. Wallet connect, transaction builders, event listeners integrados."),
            ("\U0001f45b", "Wallet Connect Soberano", "SDK de conexi\u00f3n con wallets soberanas. Soporte para browser wallets, mobile y hardware wallets."),
            ("\U0001f4bb", "Contract Interaction CLI", "CLI interactiva para llamar funciones de contratos, leer estado y debuggear transacciones desde terminal."),
            ("\u26fd", "Gas Estimation Precisa", "Estimaci\u00f3n de gas con simulaci\u00f3n previa de la transacci\u00f3n. Sin sorpresas, sin transacciones fallidas por gas."),
            ("\U0001f4e1", "Event Listener Framework", "Framework de escucha de eventos con filtros, subscripciones y handlers tipados. WebSocket y polling autom\u00e1tico."),
            ("\U0001f3d7\ufe0f", "DevNet Local Instant\u00e1neo", "Red local de desarrollo instant\u00e1nea con bloques configurables, cuentas pre-funded y estado reseteable."),
        ],
        "apis": [
            ("POST", "/api/v1/dapps/create", "Crear nuevo proyecto DApp. Params: name, template, chain, framework."),
            ("POST", "/api/v1/dapps/deploy", "Desplegar DApp. Params: project_id, chain, env, constructor_args."),
            ("GET", "/api/v1/dapps/templates", "Lista de templates disponibles con descripci\u00f3n y categor\u00eda."),
            ("POST", "/api/v1/dapps/test", "Ejecutar tests del proyecto. Params: project_id, test_suite, coverage."),
            ("GET", "/api/v1/dapps/status/{project_id}", "Estado del proyecto: tests, deploys, contratos activos."),
            ("POST", "/api/v1/dapps/interact", "Interactuar con contrato. Params: address, function, args, chain."),
        ],
        "db_stores": ["dapps-projects", "test-results", "deploy-history"],
        "arch": [
            ("var(--accent)", "DEVELOPER", "SDK \u00b7 CLI \u00b7 Templates \u00b7 IDE Plugins", "TypeScript \u00b7 Rust \u00b7 Python \u00b7 Multi-framework"),
            ("#00bcd4", "BUILD + TEST", "Compiler \u00b7 Testing \u00b7 Fuzzing \u00b7 Coverage", "Hot reload \u00b7 Time travel \u00b7 Gas profiling"),
            ("#7c4dff", "SMART CONTRACTS", "Deploy \u00b7 Verify \u00b7 Upgrade \u00b7 Monitor", "Multi-chain \u00b7 Deterministic \u00b7 Audited"),
            ("var(--accent)", "MAMEYNODE + FRONTEND", "Blockchain \u00b7 React/Vue \u00b7 Wallet \u00b7 Events", "Production ready \u00b7 PWA \u00b7 Offline-first"),
        ],
    },
]


def build_html(p):
    slug = p["dir"]
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    db_stores = p["db_stores"]

    # Build architecture diagram
    arch_layers = p["arch"]
    arch_lines = []
    dash = "\u2500"
    for i, (color, label, content, detail) in enumerate(arch_layers):
        pad_label = dash * max(1, 42 - len(label))
        pad_content = " " * max(1, 45 - len(content))
        pad_detail = " " * max(1, 45 - len(detail))
        arch_lines.append(f'<span style="color:{color}">\u250c\u2500 {label} {pad_label}\u2510</span>')
        arch_lines.append(f'\u2502  {content}{pad_content}\u2502')
        arch_lines.append(f'\u2502  {detail}{pad_detail}\u2502')
        if i < len(arch_layers) - 1:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u252c\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
            arch_lines.append('                   \u2502')
        else:
            arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
    NL = chr(10)
    arch_html = NL.join(arch_lines)

    # Stats HTML - each on its own line
    stats_lines = []
    for val, lbl in metrics:
        stats_lines.append(f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>')
    stats_html = NL.join(stats_lines)

    # Cards HTML - each card gets 3 lines
    cards_lines = []
    for cicon, ctitle, cdesc in cards:
        cards_lines.append(f'<article class="card">')
        cards_lines.append(f'<div class="card-icon" aria-hidden="true">{cicon}</div>')
        cards_lines.append(f'<h4>{ctitle}</h4>')
        cards_lines.append(f'<p>{cdesc}</p>')
        cards_lines.append(f'</article>')

    cards_html = NL.join(cards_lines)

    # API HTML - each endpoint gets 2 lines
    api_lines = []
    for method, endpoint, adesc in apis:
        color = "#ffd600" if method == "POST" else "#00FF41"
        api_lines.append(f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div>')
        api_lines.append(f'<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{adesc}</p>')

    api_html = NL.join(api_lines)

    # DB stores JSON
    stores_json = str(db_stores).replace("'", '"')

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta property="og:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle}.">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} \u2014 Ierahkwa Ne Kanienke</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header>
<div class="logo"><div class="logo-icon" aria-hidden="true">{icon}</div><h1>{title}</h1></div>
<nav aria-label="Navegacion principal">
<a href="#dashboard" aria-current="page">Dashboard</a>
<a href="#features">Modulos</a>
<a href="#api">API</a>
<a href="#pricing">Precios</a>
</nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">\u269b\ufe0f</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar M\u00f3dulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{stats_html}
</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">\U0001f3d7\ufe0f</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<div class="section-title" id="features">
<h3>M\u00f3dulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}
</div>

<div class="section" id="api">
<h2><span aria-hidden="true">\U0001f50c</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}
</div>
</div>

<div class="section-title" id="pricing">
<h3>Planes Soberanos</h3>
<p>Empieza gratis. Escala soberanamente.</p>
</div>

<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Guerrero</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">0 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Dashboard b\u00e1sico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Naci\u00f3n</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-naci\u00f3n</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">\U0001f6e1\ufe0f</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script>
(function(){{
  var DB_NAME='ierahkwa-{slug}';
  var DB_VER=1;
  var STORES={stores_json};
  var db=null;
  function openDB(){{
    return new Promise(function(resolve,reject){{
      var req=indexedDB.open(DB_NAME,DB_VER);
      req.onupgradeneeded=function(){{
        var d=req.result;
        STORES.forEach(function(s){{
          if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})
        }});
      }};
      req.onsuccess=function(){{db=req.result;resolve(db)}};
      req.onerror=function(){{reject(req.error)}}
    }})
  }}
  function showOfflineBanner(show){{
    var b=document.getElementById('offline-banner');
    if(!b){{
      b=document.createElement('div');
      b.id='offline-banner';
      b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';
      b.textContent='Modo Offline \u2014 Datos y operaciones pendientes disponibles offline.';
      document.body.appendChild(b)
    }}
    b.style.transform=show?'translateY(0)':'translateY(100%)'
  }}
  function init(){{
    openDB().then(function(){{
      window.addEventListener('online',function(){{showOfflineBanner(false)}});
      window.addEventListener('offline',function(){{showOfflineBanner(true)}});
      if(!navigator.onLine)showOfflineBanner(true);
      console.log('[{slug}] Offline module ready')
    }})
  }}
  init()
}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''
    return html


def main():
    created = []
    for p in PLATFORMS:
        dirpath = BASE / p["dir"]
        dirpath.mkdir(parents=True, exist_ok=True)
        filepath = dirpath / "index.html"
        html = build_html(p)
        filepath.write_text(html, encoding="utf-8")
        line_count = html.count('\n') + 1
        created.append((p["dir"], line_count))
        print(f"  OK {p['dir']}/index.html -- {line_count} lineas")

    print(f"\n{len(created)} plataformas blockchain creadas exitosamente.")
    print(f"Base: {BASE}")


if __name__ == "__main__":
    main()
