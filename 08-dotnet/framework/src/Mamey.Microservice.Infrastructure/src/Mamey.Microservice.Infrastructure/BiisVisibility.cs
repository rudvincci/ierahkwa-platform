using System.Runtime.CompilerServices;

// BIIS (Bank of International Indigenous Settlements) Microservices
// All Infrastructure layers for BIIS microservices

// D1-Liquidity-Pool
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLiquidityPools.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InterbankReserves.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityCorridors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReserveManagementDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityStressTests.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReservePoolAnalytics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AutomatedRebalancings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralAllocations.Infrastructure")]

// D2-Currency-Exchange
[assembly: InternalsVisibleTo("Mamey.BIIS.CurrencySwaps.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ExchangeRates.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CurrencyPairs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementFinalities.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AMLCTFChecks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InteroperabilityBridges.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SpreadConfigurations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.OrderBookEntries.Infrastructure")]

// D3-Cross-Border-Settlement
[assembly: InternalsVisibleTo("Mamey.BIIS.RTGSPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementValidations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PaymentRoutes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SovereignBondSettlements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementDisputes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.HighValueTransactions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.RepatriationTransactions.Infrastructure")]

// D4-Interbank-Payment
[assembly: InternalsVisibleTo("Mamey.BIIS.MultilateralPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryDisbursements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryFlows.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementReports.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PaymentExceptions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReconciliationRecords.Infrastructure")]

// D5-Blockchain
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLedgerEntries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TransactionHashes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AuditRecords.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DigitalSignatures.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceDecisions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.BlockchainShards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AuditIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LedgerBackups.Infrastructure")]

// D6-Asset-Collateral
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetVerifications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralTokens.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetLocks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetLiquidations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityDeployments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetPrices.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralTransfers.Infrastructure")]

// D7-Identity-Compliance
[assembly: InternalsVisibleTo("Mamey.BIIS.IdentityIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyPermissions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.MFACredentials.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ZKPCredentials.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ComplianceEvents.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.Delegations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.IdentityFederations.Infrastructure")]

// D8-Zero-Knowledge
[assembly: InternalsVisibleTo("Mamey.BIIS.ConfidentialTransactions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PrivacyFilters.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DataSovereigntyControls.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.Consents.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PrivacyCertificates.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ComplianceReports.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DataOffboardings.Infrastructure")]

// D9-Treaty-Governance
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyComplianceMonitors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyViolationAlerts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CitizenFiscalTribunalEscalations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceDecisionAudits.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyDisputeResolutions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InterNationTreatyEnforcementRegistries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceScorecards.Infrastructure")]

// D10-AI-ML-Risk
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityForecastingModels.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.RiskAnomalyDetections.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.StressTestingScenarios.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLinkedRiskGovernanceDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AdaptiveLiquidityReserveManagements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PredictiveTreatyViolationMonitorings.Infrastructure")]

// D11-Future-Enhancements
[assembly: InternalsVisibleTo("Mamey.BIIS.TokenizedSovereignBonds.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyCompliantDeFiProtocols.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CrossNationReservePoolingExtensions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.EconomicImpactAssessments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SmartTreatyContractFrameworks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLinkedCreditRatings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.UniversalWalletInteroperabilityLayers.Infrastructure")]

// API Gateway
[assembly: InternalsVisibleTo("Mamey.BIIS.API.Infrastructure")]

namespace Mamey.Microservice.Infrastructure;









