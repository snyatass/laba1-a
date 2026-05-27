namespace Laba1;

public class DeviceEventArgs : EventArgs
{
    public string DeviceName { get; }
    public string Message { get; }
 
    public DeviceEventArgs(string deviceName, string message)
    {
        DeviceName = deviceName;
        Message = message;
    }
}