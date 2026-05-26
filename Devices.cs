namespace Laba1;

public class Computer : Device, IUpsCapable, IPrintable
{
    private int _upsRemainingMinutes;
    private bool _isUsingUps;
 
    public Computer(string name, CpuInfo cpu, int ramGb, int storageGb)
        : base(name, cpu, ramGb, storageGb)
    {
        SetMainPower(true);
    }
 
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
            OnOperationFailed("ДБЖ не підключений або розряджений");
            _isUsingUps = false;
            UpdatePowerState();
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
        return _isUsingUps && _upsRemainingMinutes > 0;
    }
 
    public bool Print()
    {
        if (!CheckReady("Друк")) return false;
        if (!HasPrinter)
        {
            OnOperationFailed("принтер не підключений");
            return false;
        }
        if (!CheckSoftware("PrintDriver", "Друк")) return false;
        
        if (!IsMainPowerOn) ConsumeUpsMinutes(5); 
        return true;
    }
    
    private void DynamicUpsDrain(double hours)
    {
        if (!IsMainPowerOn && _isUsingUps && _upsRemainingMinutes > 0)
        {
            int minsToConsume = (int)(hours * 60);
            ConsumeUpsMinutes(minsToConsume);
        }
    }
    public override bool Communicate()
    {
        if (!base.Communicate()) return false;
        DynamicUpsDrain(0.2); 
        return true;
    }
    public override bool PlayGame()
    {
        if (!base.PlayGame()) return false;
        DynamicUpsDrain(1.0);
        return true;
    }

    public override bool WatchVideo()
    {
        if (!base.WatchVideo()) return false;
        DynamicUpsDrain(0.3);
        return true;
    }

    public override bool Work()
    {
        if (!base.Work()) return false;
        DynamicUpsDrain(0.3);
        return true;
    }
    public int GetUpsMinutes() => _upsRemainingMinutes;
    public bool IsActiveUps() => _isUsingUps;
}
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