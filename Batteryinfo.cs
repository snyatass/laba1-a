namespace Laba1;

public record CpuInfo(string Model, double GhzSpeed, int Cores);
 
public enum BatteryTier
{
    Small, 
    Large   
}
 
public static class BatteryRules
{
    public static double GetMaxHours(BatteryTier tier, bool intensive)
    {
        return (tier, intensive) switch
        {
            (BatteryTier.Small, false) => 48.0,
            (BatteryTier.Small, true)  => 16.0,
            (BatteryTier.Large, false) => 12.0,
            (BatteryTier.Large, true)  =>  4.0,
            _ => 0
        };
    }
 
    public static bool IsIntensiveOperation(string operation)
    {
        return operation is "PlayGame" or "WatchVideo";
    }
}