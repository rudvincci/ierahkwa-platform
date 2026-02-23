using Microsoft.Extensions.DependencyInjection;
using Mamey.AI.Government.Interfaces;
using Mamey.AI.Government.Services;
using Mamey.AI.Government.Monitoring;

namespace Mamey.AI.Government;

public static class Extensions
{
    public static IServiceCollection AddGovernmentAI(this IServiceCollection services)
    {
        // Core AI Services
        services.AddSingleton<DocumentClassifier>();
        services.AddSingleton<TamperDetector>();
        services.AddSingleton<OcrService>();
        services.AddSingleton<IDocumentVerificationService, DocumentVerificationService>();
        
        // Fraud Detection Services
        services.AddSingleton<DuplicateDetector>();
        services.AddSingleton<BehavioralAnalyzer>();
        services.AddSingleton<IFraudDetectionService, FraudDetectionService>();
        
        // KYC/AML Services
        services.AddSingleton<SanctionsScreeningService>();
        services.AddSingleton<PepDetectionService>();
        services.AddSingleton<IKycAmlService, KycAmlService>();
        
        // Biometric Services
        services.AddSingleton<FaceMatchingService>();
        services.AddSingleton<LivenessDetectionService>();
        services.AddSingleton<FingerprintService>();
        services.AddSingleton<IBiometricMatchingService, BiometricMatchingService>();
        
        // Compliance Services
        services.AddSingleton<PolicyEngine>();
        services.AddSingleton<IComplianceService, ComplianceAutomationService>();
        
        // Anomaly Detection Services
        services.AddSingleton<PatternAnalyzer>();
        services.AddSingleton<IAnomalyDetectionService, AnomalyDetectionService>();

        // Chatbot / NLP Services
        services.AddSingleton<IntentClassifier>();
        services.AddSingleton<ResponseGenerator>();
        services.AddSingleton<INLPService, ChatbotService>();
        
        // Predictive Analytics
        services.AddSingleton<ForecastingEngine>();
        services.AddSingleton<IPredictiveAnalyticsService, PredictiveAnalyticsService>();
        
        // Monitoring
        services.AddSingleton<ModelPerformanceTracker>();
        services.AddSingleton<AiMetricsService>();
        
        return services;
    }
}
