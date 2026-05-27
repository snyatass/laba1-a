namespace Laba1.Interfaces;

public interface IPeripheralHost
{
    bool HasSpeakers { get; }
    bool HasPrinter { get; }
    void ConnectSpeakers();
    void ConnectPrinter();
}