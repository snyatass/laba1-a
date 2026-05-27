using Laba1.Interfaces;
namespace Laba1;

public class Laptop : BatteryDevice, IUpsCapable, IPrintable
{
    private int _upsRemainingMinutes;
    private bool _isUsingUps;
 
    public Laptop(string name, CpuInfo cpu, int ramGb, int storageGb, int batteryCapacityMah)
        : base(name, cpu, ramGb, storageGb, batteryCapacityMah) { }
 
    public void ConnectUps(int capacityMinutes)
    {
        _upsRemainingMinutes = Math.Min(capacityMinutes, 30);
    }
 
    public bool UseUps(int minutes)
    {
        if (minutes <= 0)
        {
            _isUsingUps = false;
            UpdatePowerState();
            return true;
        }

        if (_upsRemainingMinutes <= 0)
        {
            OnOperationFailed("ДБЖ розряджений або не підключений");
            return false;
        }

        _isUsingUps = true;
        UpdatePowerState();
        return true;
    }
    public bool ConsumeUpsMinutes(int minutes)
    {
        if (!_isUsingUps || _upsRemainingMinutes <= 0) return false;
        _upsRemainingMinutes = Math.Max(0, _upsRemainingMinutes - minutes);
        if (_upsRemainingMinutes == 0)
        {
            _isUsingUps = false;
        }
        UpdatePowerState();
        return true;
    }
 
    protected override bool HasAlternativePower()
    {
        return BatteryLevelPercent > 0 || (_isUsingUps && _upsRemainingMinutes > 0);
    }
 
    public bool Print()
    {
        if (!CheckReady("Друк")) return false;
        if (!HasPrinter)
        {
            OnOperationFailed("Принтер не підключений");
            return false;
        }
        if (!CheckSoftware("PrintDriver", "Друк")) return false;
        return true;
    }
    private void DynamicDrain(double hours, bool intensive)
    {
        if (!IsMainPowerOn && _isUsingUps && _upsRemainingMinutes > 0)
        {
            int minsToConsume = (int)(hours * 60);
            ConsumeUpsMinutes(minsToConsume);
        }
        else
        {
            DrainBattery(hours, intensive);
        }
    }
    public override bool Communicate()
    {
        if (!base.Communicate()) return false;
        DynamicDrain(1.0, intensive: false); 
        return true;
    }

    public override bool ListenToMusic()
    {
        if (!base.ListenToMusic()) return false;
        DynamicDrain(1.5, intensive: false); 
        return true;
    }
    
    public override bool PlayGame()
    {
        if (!base.PlayGame()) return false; 
        DynamicDrain(1.0, intensive: true); 
        return true;
    }

    public override bool WatchVideo()
    {
        if (!base.WatchVideo()) return false;
        DynamicDrain(1.5, intensive: true); 
        return true;
    }

    public override bool Work()
    {
        if (!base.Work()) return false;
        DynamicDrain(2.0, intensive: false); 
        return true;
    }
    public int GetUpsMinutes() => _upsRemainingMinutes;
    public bool IsActiveUps() => _isUsingUps;
}