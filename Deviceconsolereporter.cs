
namespace Laba1;

public static class DeviceConsoleReporter
{
    public static void PrintDeviceStatus(Device device)
    {
        Console.WriteLine($"\n--- {device.GetType().Name}: {device.Name} ---");
        Console.WriteLine($"  Увімкнено:       {YesNo(device.IsOn)}");
        string powerStatus = "Ні (немає живлення)";
        if (device.HasPower)
        {
            if (device is Computer c && c.IsActiveUps() && !c.IsMainPowerOn)
                powerStatus = "Так (від ДБЖ)";
            else if (device is Laptop l && l.IsActiveUps() && !l.IsMainPowerOn)
                powerStatus = "Так (від ДБЖ)";
            else if (device.IsMainPowerOn) 
                powerStatus = "Так (від мережі 220V)";
            else
                powerStatus = "Так (від акумулятора)";
        }
        Console.WriteLine($"  Живлення:        {powerStatus}");
        Console.WriteLine($"  Мережа:          {YesNo(device.IsConnectedToNetwork)}");
        Console.WriteLine($"  Динаміки:        {YesNo(device.HasSpeakers)}");
        Console.WriteLine($"  Принтер:         {YesNo(device.HasPrinter)}");
        Console.WriteLine($"  Процесор:        {device.Cpu.Model} {device.Cpu.GhzSpeed} GHz x{device.Cpu.Cores}");
        Console.WriteLine($"  RAM:             {device.RamGb} GB");
        Console.WriteLine($"  Сховище:         {device.FreeStorageGb}/{device.StorageGb} GB (вільно/всього)");
        Console.WriteLine($"  ПЗ:              {string.Join(", ", device.InstalledSoftware.DefaultIfEmpty("—"))}");
 
        if (device is BatteryDevice bd)
        {
            Console.WriteLine($"  Акумулятор:      {bd.BatteryLevelPercent:F0}% ({bd.BatteryCapacityMah} мАг)");
        }

        if (device is Computer comp)
        {
            Console.WriteLine($"  Резерв ДБЖ:      {comp.GetUpsMinutes()} хв (Активний: {YesNo(comp.IsActiveUps())})");
        }
        else if (device is Laptop lap)
        {
            Console.WriteLine($"  Резерв ДБЖ:      {lap.GetUpsMinutes()} хв (Активний: {YesNo(lap.IsActiveUps())})");
        }
    }

    public static void PrintDeviceList(IReadOnlyList<Device> devices)
    {
        for (int i = 0; i < devices.Count; i++)
        {
            var d = devices[i];
            string battery = d is BatteryDevice bd ? $" | Акум: {bd.BatteryLevelPercent:F0}%" : "";
            Console.WriteLine(
                $"{i + 1}. [{d.GetType().Name}] {d.Name} | " +
                $"Увімк: {YesNo(d.IsOn)} | " +
                $"Мережа: {YesNo(d.IsConnectedToNetwork)}" +
                battery);
        }
    }
 
    public static void PrintOperationResult(string deviceName, string operation, bool success)
    {
        string status = success ? "[УСПІХ]" : "[ПОМИЛКА]";
        Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"  [{deviceName}] {operation}: {status}");
        Console.ResetColor();
    }
 
    public static void OnPowerChanged(object? sender, DeviceEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  [ЖИВЛЕННЯ] {e.DeviceName}: {e.Message}");
        Console.ResetColor();
    }
 
    public static void OnOperationFailed(object? sender, DeviceEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [ПОМИЛКА] {e.DeviceName}: {e.Message}");
        Console.ResetColor();
    }
 
    public static void OnLowBattery(object? sender, DeviceEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"  [АКУМУЛЯТОР] {e.DeviceName}: {e.Message}");
        Console.ResetColor();
    }
 
    public static void PrintHeader(string title)
    {
        Console.WriteLine($"\n=== {title} ===");
    }
 
    public static void PrintSeparator()
    {
        Console.WriteLine(new string('-', 50));
    }
 
    private static string YesNo(bool value) => value ? "Так" : "Ні";
}