namespace Mamey.AI.Government.Models;

public class ForecastResult
{
    public List<double> ForecastValues { get; set; } = new();
    public List<DateTime> Dates { get; set; } = new();
    public double ConfidenceIntervalUpper { get; set; }
    public double ConfidenceIntervalLower { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
}
