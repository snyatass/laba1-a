namespace Laba1.Interfaces;

public interface IBatteryPowerable : IPowerable
{
    int BatteryCapacityMah { get; }
    double BatteryLevelPercent { get; }
    bool ChargeBattery(double percent);
}