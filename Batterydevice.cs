namespace Laba1;

public abstract class BatteryDevice : Device, IBatteryPowerable
{
    public int BatteryCapacityMah { get; }
    public double BatteryLevelPercent { get; private set; } = 100.0;
 
    private readonly BatteryTier _tier;
 
    public event EventHandler<DeviceEventArgs>? LowBattery;
 
    protected BatteryDevice(string name, CpuInfo cpu, int ramGb, int storageGb, int batteryCapacityMah)
        : base(name, cpu, ramGb, storageGb)
    {
        BatteryCapacityMah = batteryCapacityMah;
        _tier = batteryCapacityMah is >= 2000 and <= 3000 ? BatteryTier.Small : BatteryTier.Large;
        ChargeBattery(0);
    }
 
    protected override bool HasAlternativePower()
    {
        return BatteryLevelPercent > 0;
    }
    
    public bool ChargeBattery(double percent)
    {
        if (percent < 0) return false;
        BatteryLevelPercent = Math.Min(100, BatteryLevelPercent + percent);
        UpdatePowerState();
        return true;
    }
    
    public void DrainBattery(double hours, bool intensive)
    {
        if (IsMainPowerOn) 
        {
            return; 
        }
        double maxHours = BatteryRules.GetMaxHours(_tier, intensive);
        if (maxHours <= 0) return;
        double drainPercent;
        if (!IsOn)
        {
            double slowMaxHours = maxHours * 100.0; 
            drainPercent = (hours / slowMaxHours) * 100.0;
        }
        else
        {
            drainPercent = (hours / maxHours) * 100.0; 
        }
        BatteryLevelPercent = Math.Max(0, BatteryLevelPercent - drainPercent);
        if (BatteryLevelPercent is > 0 and < 20)
        {
            LowBattery?.Invoke(this, new DeviceEventArgs(Name, $"низький заряд: {BatteryLevelPercent:F0}%"));
        }
        UpdatePowerState();
    }
    
}
