using Laba1;
using System.Text;
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var computer = new Computer(
    "Домашній ПК",
    new CpuInfo("Intel Core i5-12400", 3.6, 6),
    ramGb: 16,
    storageGb: 512);
    computer.SetMainPower(true);
    computer.ConnectUps(30);
    computer.TurnOn();

var laptop = new Laptop(
    "Ноутбук Acer",
    new CpuInfo("AMD Ryzen 5 5600U", 2.3, 6),
    ramGb: 8,
    storageGb: 256,
    batteryCapacityMah: 6000);
    laptop.SetMainPower(true);
    laptop.ConnectUps(30);
    laptop.TurnOn();

var smartphone = new Smartphone(
    "Samsung Galaxy S23",
    new CpuInfo("Snapdragon 8 Gen 2", 3.2, 8),
    ramGb: 8,
    storageGb: 128,
    batteryCapacityMah: 2500);
    smartphone.TurnOn();


computer.ConnectSpeakers();
computer.ConnectPrinter();
computer.ConnectNetwork();
computer.InstallSoftware("Office", 2);
computer.InstallSoftware("GameLauncher", 5);
computer.InstallSoftware("MusicPlayer", 1);
computer.InstallSoftware("VideoPlayer", 1);
computer.InstallSoftware("Messenger", 1);
computer.InstallSoftware("PrintDriver", 1);
computer.TurnOn();

laptop.ConnectSpeakers();
laptop.ConnectNetwork();
laptop.InstallSoftware("Office", 2);
laptop.InstallSoftware("MusicPlayer", 1);
laptop.InstallSoftware("VideoPlayer", 1);
laptop.InstallSoftware("Messenger", 1);
laptop.TurnOn();

smartphone.ConnectSpeakers();
smartphone.ConnectNetwork();
smartphone.InstallSoftware("MusicPlayer", 1);
smartphone.InstallSoftware("Messenger", 1);
smartphone.TurnOn();

var allDevices = new List<Device> { computer, laptop, smartphone };

SubscribeEvents(allDevices);
RunMenu();

void RunMenu()
{
    while (true)
    {
        Console.Clear();
        DrawHud();
        DrawMainMenu();

        switch (ReadChoice(0, 8))
        {
            case 1: ShowAllDevices(); break;
            case 2: ShowDeviceDetails(); break;
            case 3: PerformOperation(); break;
            case 4: ManageSoftware(); break;
            case 5: ManagePeripherals(); break;
            case 6: ManagePower(); break;
            case 7: DrainBatteryMenu(); break;
            case 8: ManageNetwork(); break;
            case 0: return;
        }
    }
}

void DrawHud()
{
    int on = allDevices.Count(d => d.IsOn);
    Console.WriteLine($"Пристроїв увімкнено: {on}/{allDevices.Count}");
    DeviceConsoleReporter.PrintSeparator();
}

void DrawMainMenu()
{
    Console.WriteLine("Оберіть дію:");
    Console.WriteLine("1. Список усіх пристроїв");
    Console.WriteLine("2. Детальний стан пристрою");
    Console.WriteLine("3. Виконати операцію на пристрої");
    Console.WriteLine("4. Встановити ПЗ");
    Console.WriteLine("5. Підключити периферію");
    Console.WriteLine("6. Керування живленням (вкл/вимк, електрика, ДБЖ)");
    Console.WriteLine("7. Розрядити акумулятор (симуляція часу)");
    Console.WriteLine("8. Керування мережею");
    Console.WriteLine("0. Вийти");
    Console.WriteLine();
}

void ShowAllDevices()
{
    Console.Clear();
    DeviceConsoleReporter.PrintHeader("СПИСОК ПРИСТРОЇВ");
    DeviceConsoleReporter.PrintDeviceList(allDevices);
    Pause();
}

void ShowDeviceDetails()
{
    var device = SelectDevice("Оберіть пристрій для перегляду");
    if (device is null) return;
    Console.Clear();
    DeviceConsoleReporter.PrintHeader($"СТАН: {device.Name}");
    DeviceConsoleReporter.PrintDeviceStatus(device);
    Pause();
}

void PerformOperation()
{
    var device = SelectDevice("Оберіть пристрій");
    if (device is null) return;

    while (true)
    {
        Console.Clear();
        DeviceConsoleReporter.PrintHeader($"ОПЕРАЦІЇ: {device.Name}");
        DeviceConsoleReporter.PrintDeviceStatus(device);
        Console.WriteLine();
        Console.WriteLine("1. Робота (Office)");
        Console.WriteLine("2. Гра");
        Console.WriteLine("3. Спілкування");
        Console.WriteLine("4. Слухати музику");
        Console.WriteLine("5. Дивитися відео");

        bool hasChat = device is Smartphone;
        bool hasPrint = device is Computer or Laptop;

        if (hasChat) Console.WriteLine("6. Чат / Соцмережі");
        if (hasPrint) Console.WriteLine("7. Друкувати");
        Console.WriteLine("0. Назад");
        Console.WriteLine();

        int max = 5;
        if (hasChat) max = 6;
        if (hasPrint) max = Math.Max(max, 7);

        int choice = ReadChoice(0, max);
        if (choice == 0) break;

        Console.WriteLine();
        switch (choice)
        {
            case 1: DeviceConsoleReporter.PrintOperationResult(device.Name, "Робота", device.Work()); break;
            case 2: DeviceConsoleReporter.PrintOperationResult(device.Name, "Гра", device.PlayGame()); break;
            case 3: DeviceConsoleReporter.PrintOperationResult(device.Name, "Спілкування", device.Communicate()); break;
            case 4: DeviceConsoleReporter.PrintOperationResult(device.Name, "Музика", device.ListenToMusic()); break;
            case 5: DeviceConsoleReporter.PrintOperationResult(device.Name, "Відео", device.WatchVideo()); break;
            case 6 when device is Smartphone sp:
                DeviceConsoleReporter.PrintOperationResult(device.Name, "Чат", sp.Chat()); break;
            case 7 when device is Computer comp:
                DeviceConsoleReporter.PrintOperationResult(device.Name, "Друк", comp.Print()); break;
            case 7 when device is Laptop lap:
                DeviceConsoleReporter.PrintOperationResult(device.Name, "Друк", lap.Print()); break;
        }
        Pause();
    }
}
void ManageSoftware()
{
    var device = SelectDevice("Оберіть пристрій для встановлення ПЗ");
    if (device is null) return;

    Console.Clear();
    DeviceConsoleReporter.PrintHeader($"ПЗ: {device.Name}");
    Console.WriteLine($"Встановлено: {string.Join(", ", device.InstalledSoftware.DefaultIfEmpty("—"))}");
    Console.WriteLine($"Вільно: {device.FreeStorageGb} GB");
    DeviceConsoleReporter.PrintSeparator();
    
    var softwareMarket = new List<(string Name, int Size)>
    {
        ("Office", 2),
        ("GameLauncher", 5),
        ("Messenger", 1),
        ("MusicPlayer", 1),
        ("VideoPlayer", 1)
    };
    
    if (device is Computer or Laptop)
    {
        softwareMarket.Add(("PrintDriver", 1));
    }

    Console.WriteLine("Оберіть ПЗ для встановлення:");
    for (int i = 0; i < softwareMarket.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {softwareMarket[i].Name} ({softwareMarket[i].Size} GB)");
    }
    Console.WriteLine("0. Назад");
    Console.WriteLine();
    
    int choice = ReadChoice(0, softwareMarket.Count);
    if (choice == 0) return;
    var selectedSoft = softwareMarket[choice - 1];
    if (device.HasSoftware(selectedSoft.Name))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n[УВАГА] {selectedSoft.Name} вже встановлено на {device.Name}!");
        Console.ResetColor();
        Pause();
        return;
    }
    Console.WriteLine();
    bool ok = device.InstallSoftware(selectedSoft.Name, selectedSoft.Size);
    DeviceConsoleReporter.PrintOperationResult(device.Name, $"Встановлення '{selectedSoft.Name}'", ok);
    Pause();
}
void ManagePeripherals()
{
    var device = SelectDevice("Оберіть пристрій");
    if (device is null) return;

    Console.Clear();
    DeviceConsoleReporter.PrintHeader($"ПЕРИФЕРІЯ: {device.Name}");
    Console.WriteLine($"Динаміки/навушники: {(device.HasSpeakers ? "підключено" : "не підключено")}");
    Console.WriteLine($"Принтер:            {(device.HasPrinter ? "підключено" : "не підключено")}");
    Console.WriteLine();
    Console.WriteLine("1. Підключити динаміки/навушники | Відключити динаміки/навушники");
    Console.WriteLine("2. Підключити принтер | Відключити принтер");
    Console.WriteLine("0. Назад");

    switch (ReadChoice(0, 2))
    {
        case 1: 
            if (device.HasSpeakers)
            {
                device.DisconnectSpeakers();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Динаміки успішно ВІДКЛЮЧЕНО.");
                Console.ResetColor();
            }
            else
            {
                device.ConnectSpeakers();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Динаміки успішно ПІДКЛЮЧЕНО.");
                Console.ResetColor();
            }
            break;
        case 2: 
            if (device is Computer or Laptop)
            {
                if (device.HasPrinter)
                {
                    device.DisconnectPrinter(); // Отключаем
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Принтер успішно ВІДКЛЮЧЕНО.");
                    Console.ResetColor();
                }
                else
                {
                    device.ConnectPrinter(); // Подключаем
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Принтер успішно ПІДКЛЮЧЕНО.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ПОМИЛКА] До пристрою {device.Name} неможливо підключити принтер!");
                Console.ResetColor();
            }
            break;
    }
    Pause();
}
void ManagePower()
{
    var device = SelectDevice("Оберіть пристрій");
    if (device is null) return;

    Console.Clear();
    DeviceConsoleReporter.PrintHeader($"ЖИВЛЕННЯ: {device.Name}");
    Console.WriteLine($"Увімкнено:   {(device.IsOn ? "Так" : "Ні")}");
    Console.WriteLine($"Є живлення:  {(device.HasPower ? "Так" : "Ні")}");
    Console.WriteLine();
    Console.WriteLine("1. Увімкнути пристрій");
    Console.WriteLine("2. Вимкнути пристрій");
    Console.WriteLine("3. Відімкнути електрику (симуляція)");
    Console.WriteLine("4. Підключити електрику");
    if (device is Computer or Laptop) Console.WriteLine("5. Керування ДБЖ (увімк/вимк резерв)");
    Console.WriteLine("0. Назад");

    int max = (device is Computer or Laptop) ? 5 : 4;
    switch (ReadChoice(0, max))
    {
        case 1:
            DeviceConsoleReporter.PrintOperationResult(device.Name, "Увімкнення", device.TurnOn());
            break;
        case 2:
            DeviceConsoleReporter.PrintOperationResult(device.Name, "Вимкнення", device.TurnOff());
            break;
        case 3:
            device.SetMainPower(false);
            Console.WriteLine("Електрику відключено.");
            break;
        case 4:
            device.SetMainPower(true);
            Console.WriteLine("Електрику підключено.");
            if (device is BatteryDevice bd)
            {
                double neededCharge = 100.0 - bd.BatteryLevelPercent;
                bd.ChargeBattery(neededCharge);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[ЗАРЯДКА] Акумулятор миттєво заряджено до 100%!");
                Console.ResetColor();
            }
            break;
        case 5 when device is Computer comp:
            Console.WriteLine("1. Увімкнути роботу від ДБЖ");
            Console.WriteLine("2. Вимкнути ДБЖ");
            int upsCompChoice = ReadChoice(1, 2);
            // Если выбрали 1 — передаем 30 минут, если 2 — передаем 0 (выключение)
            DeviceConsoleReporter.PrintOperationResult(device.Name, "Режим ДБЖ", comp.UseUps(upsCompChoice == 1 ? 30 : 0));
            break;
        case 5 when device is Laptop lap:
            Console.WriteLine("1. Увімкнути роботу від ДБЖ (економити батарею)");
            Console.WriteLine("2. Вимкнути ДБЖ (перейти на батарею)");
            int upsLapChoice = ReadChoice(1, 2);
            DeviceConsoleReporter.PrintOperationResult(device.Name, "Режим ДБЖ", lap.UseUps(upsLapChoice == 1 ? 30 : 0));
            break;
    }
    Pause();
}

void DrainBatteryMenu()
{
    var batteryDevices = allDevices.OfType<BatteryDevice>().ToList();
    if (batteryDevices.Count == 0)
    {
        Console.WriteLine("Немає пристроїв з акумулятором.");
        Pause();
        return;
    }

    Console.Clear();
    DeviceConsoleReporter.PrintHeader("СИМУЛЯЦІЯ РОЗРЯДУ АКУМУЛЯТОРА");
    for (int i = 0; i < batteryDevices.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {batteryDevices[i].Name} — {batteryDevices[i].BatteryLevelPercent:F0}%");
    }
    Console.Write("\nОберіть пристрій (0 — назад): ");
    if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > batteryDevices.Count) { Pause(); return; }

    var bd = batteryDevices[idx - 1];

    Console.Write("Скільки годин минуло? ");
    if (!double.TryParse(Console.ReadLine(), out double hours) || hours <= 0) { Pause(); return; }

    Console.Write("Інтенсивне використання (ігри/відео)? (т/н): ");
    bool intensive = Console.ReadLine()?.Trim().ToLower() is "т" or "y";

    bd.DrainBattery(hours, intensive);
    Console.WriteLine($"\nАкумулятор після симуляції: {bd.BatteryLevelPercent:F0}%");
    Pause();
}

void ManageNetwork()
{
    var device = SelectDevice("Оберіть пристрій");
    if (device is null) return;

    Console.Clear();
    DeviceConsoleReporter.PrintHeader($"МЕРЕЖА: {device.Name}");
    Console.WriteLine($"Поточний стан: {(device.IsConnectedToNetwork ? "підключено" : "відключено")}");
    Console.WriteLine("1. Підключити мережу");
    Console.WriteLine("2. Відключити мережу");
    Console.WriteLine("0. Назад");

    switch (ReadChoice(0, 2))
    {
        case 1: device.ConnectNetwork(); Console.WriteLine("Мережу підключено."); break;
        case 2: device.DisconnectNetwork(); Console.WriteLine("Мережу відключено."); break;
    }
    Pause();
}

Device? SelectDevice(string title)
{
    while (true)
    {
        Console.Clear();
        DeviceConsoleReporter.PrintHeader(title);
        DeviceConsoleReporter.PrintDeviceList(allDevices);
        Console.WriteLine("0. Скасувати");
        Console.Write("\nВведіть номер: ");

        if (int.TryParse(Console.ReadLine(), out int idx))
        {
            if (idx == 0) return null;
            if (idx >= 1 && idx <= allDevices.Count) return allDevices[idx - 1];
        }
        Console.WriteLine("Невірний вибір.");
    }
}

int ReadChoice(int min, int max)
{
    while (true)
    {
        Console.Write("Ваш вибір: ");
        if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max)
            return v;
        Console.WriteLine($"Введіть число від {min} до {max}.");
    }
}

void Pause()
{
    Console.WriteLine();
    Console.Write("Натисніть Enter щоб продовжити...");
    Console.ReadLine();
}

void SubscribeEvents(IEnumerable<Device> devices)
{
    foreach (var d in devices)
    {
        d.PowerChanged += DeviceConsoleReporter.OnPowerChanged;
        d.OperationFailed += DeviceConsoleReporter.OnOperationFailed;
        if (d is BatteryDevice bd)
            bd.LowBattery += DeviceConsoleReporter.OnLowBattery;
    }
}