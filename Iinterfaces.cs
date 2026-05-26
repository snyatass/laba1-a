namespace Laba1;

public interface IWorkable
{
    bool Work();
}
 
public interface IGameable
{
    bool PlayGame();
}
 
public interface ICommunicable
{
    bool Communicate();
}
 
public interface IMusicPlayable
{
    bool ListenToMusic();
}
 
public interface IVideoPlayable
{
    bool WatchVideo();
}
public interface IPowerable
{
    bool HasPower { get; }
    bool TurnOn();
    bool TurnOff();
}
 
public interface IBatteryPowerable : IPowerable
{
    int BatteryCapacityMah { get; }
    double BatteryLevelPercent { get; }
    bool ChargeBattery(double percent);
}
 
public interface IUpsCapable
{
    bool UseUps(int minutes);
}
public interface IHasCpu
{
    CpuInfo Cpu { get; }
}
 
public interface IHasRam
{
    int RamGb { get; }
}
 
public interface IHasStorage
{
    int StorageGb { get; }
    int FreeStorageGb { get; }
    bool InstallSoftware(string name, int sizeGb);
}
public interface ISoftwareHost
{
    bool HasSoftware(string name);
}
 
public interface INetworkCapable
{
    bool IsConnectedToNetwork { get; }
    void ConnectNetwork();
    void DisconnectNetwork();
}
public interface IPeripheralHost
{
    bool HasSpeakers { get; }
    bool HasPrinter { get; }
    void ConnectSpeakers();
    void ConnectPrinter();
}
public interface IPrintable
{
    bool Print();
}