namespace Laba1.Interfaces;

public interface INetworkCapable
{
    bool IsConnectedToNetwork { get; }
    void ConnectNetwork();
    void DisconnectNetwork();
}