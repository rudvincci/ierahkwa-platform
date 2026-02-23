namespace Mamey;

public static class UnitsOfMeasurementExtensions
{
    public static FeetInches ConvertInchesToFeetAndInches(this double totalInches)
        => new FeetInches { Feet = (int)(totalInches / 12), Inches = (int)(totalInches % 12) };
    public static double ConvertKgToLbs(this double kg) => kg * 2.20462;
    public static double ConvertLbsToKg(this double lbs) => lbs / 2.20462;
    public static double ConvertFtInToCm(int feet, int inches) => (feet * 12 + inches) * 2.54;
    public static (int feet, int inches) ConvertCmToFtIn(this double cm)
    {
        var totalInches = cm / 2.54;
        var feet = (int)totalInches / 12;
        var inches = (int)totalInches % 12;
        return (feet, inches);
    }
    public static double ConvertCmToInches(this double cm) => cm / 2.54;
    public static double ConvertInchesToCm(this double inches) => inches * 2.54;
    public static double ConvertFtInToInches(int feet, int inches) => feet * 12 + inches;
    public static double ConvertMetersToInches(this double meters) => meters * 39.37007874;
    public static double ConvertInchesToMeters(this double inches) => inches / 39.37007874;
}
public struct FeetInches
{
    public int Feet { get; set; }
    public int Inches { get; set; }
    public override string ToString()
    {
        return $"{Feet}'{Inches}\"";
    }
}
