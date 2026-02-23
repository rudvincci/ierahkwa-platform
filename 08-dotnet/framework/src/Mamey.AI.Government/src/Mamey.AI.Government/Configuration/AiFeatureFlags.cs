namespace Mamey.AI.Government.Configuration;

public class AiFeatureFlags
{
    public bool EnableDocumentVerification { get; set; } = true;
    public bool EnableFraudDetection { get; set; } = true;
    public bool EnableKycAutomation { get; set; } = true;
    public bool EnableBiometrics { get; set; } = false; // Default off for sensitive feature
    public bool EnableChatbot { get; set; } = false;
    public bool EnableAnomalyDetection { get; set; } = true;
    public bool EnablePredictiveAnalytics { get; set; } = false;
    
    // Advanced flags
    public double FraudDetectionRolloutPercentage { get; set; } = 100.0;
    public bool UseMockModels { get; set; } = true; // For development
}
