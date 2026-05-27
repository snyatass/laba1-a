using Laba1.Interfaces;
namespace Laba1;

public abstract class Device :
    IPowerable,
    IHasCpu, IHasRam, IHasStorage,
    ISoftwareHost, INetworkCapable, IPeripheralHost,
    IWorkable, IGameable, ICommunicable, IMusicPlayable, IVideoPlayable
{
    public bool IsMainPowerOn { get; private set; } 
    public bool HasPower { get; private set; }
    public bool IsOn { get; private set; }
    public CpuInfo Cpu { get; }
    public int RamGb { get; }
    public int StorageGb { get; }
    public int FreeStorageGb { get; private set; }
    public bool HasSpeakers { get; private set; }
    public bool HasPrinter { get; private set; }
    public bool IsConnectedToNetwork { get; private set; }
    
    private readonly List<string> _installedSoftware = new();
    public event EventHandler<DeviceEventArgs>? PowerChanged;
    public event EventHandler<DeviceEventArgs>? OperationFailed;
 
    public string Name { get; }
 
    protected Device(string name, CpuInfo cpu, int ramGb, int storageGb)
    {
        Name = name;
        Cpu = cpu;
        RamGb = ramGb;
        StorageGb = storageGb;
        FreeStorageGb = storageGb;
        IsMainPowerOn = false;
        HasPower = false;
        IsOn = false;
    }
 
    public void SetMainPower(bool available)
    {
        IsMainPowerOn = available;
        UpdatePowerState();
    }
 
    protected virtual void UpdatePowerState()
    {
        bool previousHasPower = HasPower;
        HasPower = IsMainPowerOn || HasAlternativePower();
 
        if (HasPower != previousHasPower)
        {
            if (!HasPower && IsOn)
            {
                IsOn = false;
            }
            PowerChanged?.Invoke(this, new DeviceEventArgs(Name,
                HasPower ? "отримав живлення" : "втратив живлення"));
        }
    }
 
    protected virtual bool HasAlternativePower() => false;
 
    public bool TurnOn()
    {
        if (!HasPower)
        {
            OperationFailed?.Invoke(this, new DeviceEventArgs(Name, "немає живлення"));
            return false;
        }
        IsOn = true;
        return true;
    }
 
    public bool TurnOff()
    {
        IsOn = false;
        return true;
    }
 
    public bool InstallSoftware(string name, int sizeGb)
    {
        if (FreeStorageGb < sizeGb)
        {
            OnOperationFailed($"недостатньо місця для встановлення {name}");
            return false;
        }
        _installedSoftware.Add(name.ToLower());
        FreeStorageGb -= sizeGb;
        return true;
    }
 
    public bool HasSoftware(string name) => _installedSoftware.Contains(name);
    public IReadOnlyCollection<string> InstalledSoftware => _installedSoftware;
    
    public void ConnectNetwork() => IsConnectedToNetwork = true;
    public void DisconnectNetwork() => IsConnectedToNetwork = false;
    public void ConnectSpeakers() => HasSpeakers = true;
    public void DisconnectSpeakers() => HasSpeakers = false;
    public void ConnectPrinter() => HasPrinter = true;
    public void DisconnectPrinter() => HasPrinter = false;
    protected bool CheckReady(string operation)
    {
        if (!IsOn)
        {
            OperationFailed?.Invoke(this, new DeviceEventArgs(Name, $"пристрій вимкнено ({operation})"));
            return false;
        }
        return true;
    }
 
    public bool CheckSoftware(string name, string operation)
    {
        if (!_installedSoftware.Contains(name.ToLower())) 
        {
            OnOperationFailed($"немає ПЗ {name}");
            return false;
        }
        return true;
    }
 
    protected bool CheckNetwork(string operation)
    {
        if (!IsConnectedToNetwork)
        {
            OperationFailed?.Invoke(this, new DeviceEventArgs(Name, $"відсутнє підключення до мережі ({operation})"));
            return false;
        }
        return true;
    }
 
    protected bool CheckSpeakers(string operation)
    {
        if (!HasSpeakers)
        {
            OperationFailed?.Invoke(this, new DeviceEventArgs(Name, $"не підключено динаміки/навушники ({operation})"));
            return false;
        }
        return true;
    }

    public virtual bool Work()
    {
        if (!CheckReady("Робота")) return false;
        if (!CheckSoftware("Office", "Робота")) return false;
        return true;
    }
 
    public virtual bool PlayGame()
    {
        if (!CheckReady("Гра")) return false;
        if (!CheckSoftware("GameLauncher", "Гра")) return false;
        return true;
    }
 
    public virtual bool Communicate()
    {
        if (!CheckReady("Спілкування")) return false;
        if (!CheckNetwork("Спілкування")) return false;
        if (!CheckSoftware("Messenger", "Спілкування")) return false;
        return true;
    }
 
    public virtual bool ListenToMusic()
    {
        if (!CheckReady("Музика")) return false;
        if (!CheckSpeakers("Музика")) return false;
        if (!CheckSoftware("MusicPlayer", "Музика")) return false;
        return true;
    }
 
    public virtual bool WatchVideo()
    {
        if (!CheckReady("Відео")) return false;
        if (!CheckSpeakers("Відео")) return false;
        if (!CheckSoftware("VideoPlayer", "Відео")) return false;
        return true;
    }
    protected void OnOperationFailed(string message)
    {
        OperationFailed?.Invoke(this, new DeviceEventArgs(Name, message));
    }
    
}