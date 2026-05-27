using Laba1.Interfaces;
namespace Laba1;

public class Smartphone : BatteryDevice
{
    public Smartphone(string name, CpuInfo cpu, int ramGb, int storageGb, int batteryCapacityMah)
        : base(name, cpu, ramGb, storageGb, batteryCapacityMah) { }
        
    public override bool Communicate()
    {
        if (!CheckReady("Спілкування")) return false;
        DrainBattery(0.5, intensive: false);
        return true;
    }
    public bool Chat()
    {
        if (!CheckReady("Чат")) return false;
        if (!CheckNetwork("Чат")) return false;
        if (!CheckSoftware("Messenger", "Чат")) return false;
        DrainBattery(0.5, intensive: false);
        return true;
    }
    public override bool PlayGame()
    {
        if (!base.PlayGame()) return false;
        DrainBattery(1.5, intensive: true);
        return true;
    }
    public override bool WatchVideo()
    {
        if (!base.WatchVideo()) return false;
        DrainBattery(1.0, intensive: true); 
        return true;
    }
    public override bool Work()
    {
        if (!base.Work()) return false;
        DrainBattery(1.0, intensive: false); 
        return true;
    }
    public override bool ListenToMusic()
    {
        if (!base.ListenToMusic()) return false;
        DrainBattery(0.5, intensive: false); 
        return true;
    }
}