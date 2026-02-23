using System.Runtime.CompilerServices;

// FBDETB (Future BDET Bank) Microservices
// All Infrastructure layers for FBDETB microservices

// DA-Identity-Access-Trust
[assembly: InternalsVisibleTo("Mamey.FBDETB.IdentityIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RoleBasedAccessControls.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BiometricAuthentications.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustGuardianAssignments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ProtectionStatuses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.KYCAMLEnforcements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.GuardianProxyDelegations.Infrastructure")]

// DB-Account-Wallet-Management
[assembly: InternalsVisibleTo("Mamey.FBDETB.PersonalAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MinistryCooperativeAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TimeDepositSavings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanPoolAccounts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.WalletSynchronizations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.OfflineWalletSyncs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AccountPolicyRules.Infrastructure")]

// DC-Card-Services-Terminal
[assembly: InternalsVisibleTo("Mamey.FBDETB.VirtualCards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BiometricSmartCards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.PhysicalCardManufacturings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MobileTerminalAuths.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TrustCardActivations.Infrastructure")]

// DD-Payments-Settlements
[assembly: InternalsVisibleTo("Mamey.FBDETB.PeerToPeerPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MerchantPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.GovernmentDisbursements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RecurringPayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.POSSettlements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.InterbankTransfers.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MultiCurrencyWallets.Infrastructure")]

// DE-Lending-Credit
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignBackedLoans.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.Microloans.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LoanForgivenesses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CreditRiskEvaluations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SmartContractRepayments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CollateralRegistries.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LoanLifecycleVisibilities.Infrastructure")]

// DF-Exchange-Treasury
[assembly: InternalsVisibleTo("Mamey.FBDETB.CurrencyExchanges.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TreatyCurrencySwaps.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BondManagements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LedgerRegistrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.LiquidityCorridorAccesses.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignReserveCustodianships.Infrastructure")]

// DG-Compliance-Security
[assembly: InternalsVisibleTo("Mamey.FBDETB.AMLCTFFlaggings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AntiInflationEnforcements.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.DynamicFeeRoutings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.FraudDetections.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ZKComplianceDashboards.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ImmutableAuditTrails.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.RealTimeRedFlagBroadcastings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AccountFreezeRevokes.Infrastructure")]

// DH-Merchant-Commercial
[assembly: InternalsVisibleTo("Mamey.FBDETB.MerchantPOSIntegrations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.VendorRevenueTrackings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.WholesaleFinancings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.EscrowSmartContracts.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CrossBorderVendorClearances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.BusinessAnalytics.Infrastructure")]

// DI-Insurance-Risk
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanMutualAssurances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SovereignInsurances.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SmartContractPremiums.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.DisasterResponseTriggers.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CulturalAssetCoverages.Infrastructure")]

// DJ-Asset-Tokenization
[assembly: InternalsVisibleTo("Mamey.FBDETB.LandTrustTokenizations.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AssetTokenMarkets.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TokenEscrowCollaterals.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.TreatyCertifiedTokenProofs.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.AssetTokenMappings.Infrastructure")]

// DK-Infrastructure-Resilience
[assembly: InternalsVisibleTo("Mamey.FBDETB.MobileShipborneTerminals.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.OfflineMicrodatacenters.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.MeshNetworkBankings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.SatelliteBankings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.HybridCloudDeployments.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ZeroDowntimeUpgrades.Infrastructure")]

// DL-Citizen-Experience
[assembly: InternalsVisibleTo("Mamey.FBDETB.CitizenFinancialHealths.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.PublicTrustMetrics.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ComplaintDisputeRoutings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ClanFeedbackLoops.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.CitizenVotings.Infrastructure")]
[assembly: InternalsVisibleTo("Mamey.FBDETB.ElderAccessModes.Infrastructure")]

// DM-SDK-Integration
[assembly: InternalsVisibleTo("Mamey.FBDETB.SDKs.Infrastructure")]

// API Gateway
[assembly: InternalsVisibleTo("Mamey.FBDETB.API.Infrastructure")]

namespace Mamey.Microservice.Infrastructure;









