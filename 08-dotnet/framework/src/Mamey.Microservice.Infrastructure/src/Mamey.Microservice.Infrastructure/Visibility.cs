using System.Runtime.CompilerServices;

// Bank
[assembly: InternalsVisibleTo("Mamey.Bank.Ktt.Infrastructure")]

// Government
[assembly: InternalsVisibleTo("Mamey.Government.Citizens.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Government.Diplomats.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Government.Notifications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.Government.Passports.Infrastructure")]

// FutureWampumID (FWID) Microservices
[assembly: InternalsVisibleTo("Mamey.FWID.AccessControls.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FWID.Credentials.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FWID.DIDs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FWID.ZKPs.Infrastructure")]

// BIIS Services (Bank of International Indigenous Settlements)
// Domain 1: D1-Liquidity-Pool
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLiquidityPools.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InterbankReserves.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityCorridors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReserveManagementDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityStressTests.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReservePoolAnalytics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AutomatedRebalancings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralAllocations.Infrastructure")]

// Domain 2: D2-Currency-Exchange
[assembly: InternalsVisibleTo("Mamey.BIIS.CurrencySwaps.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ExchangeRates.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CurrencyPairs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementFinalities.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AMLCTFChecks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InteroperabilityBridges.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SpreadConfigurations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.OrderBookEntries.Infrastructure")]

// Domain 3: D3-Cross-Border-Settlement
[assembly: InternalsVisibleTo("Mamey.BIIS.RTGSPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementValidations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PaymentRoutes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SovereignBondSettlements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementDisputes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.HighValueTransactions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.RepatriationTransactions.Infrastructure")]

// Domain 4: D4-Interbank-Payment
[assembly: InternalsVisibleTo("Mamey.BIIS.MultilateralPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryDisbursements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreasuryFlows.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SettlementReports.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PaymentExceptions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ReconciliationRecords.Infrastructure")]

// Domain 5: D5-Blockchain
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLedgerEntries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TransactionHashes.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AuditRecords.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DigitalSignatures.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceDecisions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.BlockchainShards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AuditIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LedgerBackups.Infrastructure")]

// Domain 6: D6-Asset-Collateral
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetVerifications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralTokens.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetLocks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetLiquidations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityDeployments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AssetPrices.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CollateralTransfers.Infrastructure")]

// Domain 7: D7-Identity-Compliance
[assembly: InternalsVisibleTo("Mamey.BIIS.IdentityIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyPermissions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.MFACredentials.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ZKPCredentials.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ComplianceEvents.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.Delegations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.IdentityFederations.Infrastructure")]

// Domain 8: D8-Zero-Knowledge
[assembly: InternalsVisibleTo("Mamey.BIIS.ConfidentialTransactions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PrivacyFilters.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DataSovereigntyControls.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.Consents.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PrivacyCertificates.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.ComplianceReports.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.DataOffboardings.Infrastructure")]

// Domain 9: D9-Treaty-Governance
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyComplianceMonitors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyViolationAlerts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CitizenFiscalTribunalEscalations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceDecisionAudits.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyDisputeResolutions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.InterNationTreatyEnforcementRegistries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.GovernanceScorecards.Infrastructure")]

// Domain 10: D10-AI-ML-Risk
[assembly: InternalsVisibleTo("Mamey.BIIS.LiquidityForecastingModels.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.RiskAnomalyDetections.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.StressTestingScenarios.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLinkedRiskGovernanceDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.AdaptiveLiquidityReserveManagements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.PredictiveTreatyViolationMonitorings.Infrastructure")]

// Domain 11: D11-Future-Enhancements
[assembly: InternalsVisibleTo("Mamey.BIIS.TokenizedSovereignBonds.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyCompliantDeFiProtocols.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.CrossNationReservePoolingExtensions.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.EconomicImpactAssessments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.SmartTreatyContractFrameworks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.TreatyLinkedCreditRatings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.BIIS.UniversalWalletInteroperabilityLayers.Infrastructure")]

// BIIS API Gateway
[assembly: InternalsVisibleTo("Mamey.BIIS.API.Infrastructure")]

// FBDETB Services (Future BDET Bank) - 76 services across 13 domains
// Domain A: DA-Identity-Access-Trust
[assembly: InternalsVisibleTo("Mamey.FBDETB.IdentityIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RoleBasedAccessControls.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BiometricAuthentications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustGuardianAssignments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ProtectionStatuses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.KYCAMLEnforcements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.GuardianProxyDelegations.Infrastructure")]

// Domain B: DB-Account-Wallet-Management
[assembly: InternalsVisibleTo("Mamey.FBDETB.PersonalAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MinistryCooperativeAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TimeDepositSavings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanPoolAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.WalletSynchronizations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.OfflineWalletSyncs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AccountPolicyRules.Infrastructure")]

// Domain C: DC-Card-Services-Terminal
[assembly: InternalsVisibleTo("Mamey.FBDETB.VirtualCards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BiometricSmartCards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.PhysicalCardManufacturings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MobileTerminalAuths.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustCardActivations.Infrastructure")]

// Domain D: DD-Payments-Settlements
[assembly: InternalsVisibleTo("Mamey.FBDETB.PeerToPeerPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MerchantPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.GovernmentDisbursements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RecurringPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.POSSettlements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.InterbankTransfers.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MultiCurrencyWallets.Infrastructure")]

// Domain E: DE-Lending-Credit
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignBackedLoans.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.Microloans.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LoanForgivenesses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CreditRiskEvaluations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SmartContractRepayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CollateralRegistries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LoanLifecycleVisibilities.Infrastructure")]

// Domain F: DF-Exchange-Treasury
[assembly: InternalsVisibleTo("Mamey.FBDETB.CurrencyExchanges.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TreatyCurrencySwaps.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BondManagements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LedgerRegistrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LiquidityCorridorAccesses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignReserveCustodianships.Infrastructure")]

// Domain G: DG-Compliance-Security
[assembly: InternalsVisibleTo("Mamey.FBDETB.AMLCTFFlaggings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AntiInflationEnforcements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.DynamicFeeRoutings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.FraudDetections.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ZKComplianceDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ImmutableAuditTrails.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RealTimeRedFlagBroadcastings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AccountFreezeRevokes.Infrastructure")]

// Domain H: DH-Merchant-Commercial
[assembly: InternalsVisibleTo("Mamey.FBDETB.MerchantPOSIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.VendorRevenueTrackings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.WholesaleFinancings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.EscrowSmartContracts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CrossBorderVendorClearances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BusinessAnalytics.Infrastructure")]

// Domain I: DI-Insurance-Risk
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanMutualAssurances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignInsurances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SmartContractPremiums.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.DisasterResponseTriggers.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CulturalAssetCoverages.Infrastructure")]

// Domain J: DJ-Asset-Tokenization
[assembly: InternalsVisibleTo("Mamey.FBDETB.LandTrustTokenizations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AssetTokenMarkets.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TokenEscrowCollaterals.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TreatyCertifiedTokenProofs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AssetTokenMappings.Infrastructure")]

// Domain K: DK-Infrastructure-Resilience
[assembly: InternalsVisibleTo("Mamey.FBDETB.MobileShipborneTerminals.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.OfflineMicrodatacenters.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MeshNetworkBankings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SatelliteBankings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.HybridCloudDeployments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ZeroDowntimeUpgrades.Infrastructure")]

// Domain L: DL-Citizen-Experience
[assembly: InternalsVisibleTo("Mamey.FBDETB.CitizenFinancialHealths.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.PublicTrustMetrics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ComplaintDisputeRoutings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanFeedbackLoops.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CitizenVotings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ElderAccessModes.Infrastructure")]

// Domain M: DM-SDK-Integration
[assembly: InternalsVisibleTo("Mamey.FBDETB.SDKs.Infrastructure")]

// FBDETB API Gateway
[assembly: InternalsVisibleTo("Mamey.FBDETB.API.Infrastructure")]

// HolisticMedicine Services (21 services)
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Products.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Inventories.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Patients.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Practitioners.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.POS.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Compliances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Orders.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Deliveries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Auth.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Payments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Analytics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Marketings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Vendors.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Forecastings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Recommendations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Feedbacks.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Securities.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Educations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Notifications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.Operations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.HolisticMedicine.API.Infrastructure")]

// Template - REPLACE THIS LINE when creating a new microservice
// Pattern: [assembly: InternalsVisibleTo("Mamey.{Domain}.{ServiceName}.Infrastructure")]
// Example: [assembly: InternalsVisibleTo("Mamey.Government.Passports.Infrastructure")]
// CRITICAL: This MUST be updated every time a new microservice is created
[assembly: InternalsVisibleTo("Mamey.ServiceName.Infrastructure")]

namespace Mamey.Microservice.Infrastructure;
