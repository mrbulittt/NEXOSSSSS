using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using System.Drawing;


namespace NEXOS
{
    public class Kernel : Sys.Kernel
    {

        // Поле для файловой системы
        private CosmosVFS fs;
        // Текущая директория (по умолчанию корень диска 0)
        private string currentDirectory = @"0:\";
        // Инициализация перед запуском ядра
        private bool isRegistered = false;
        private string registeredUser = "";
        private string registeredPassword = "";
        public static Canvas _canvas;
        

        protected override void BeforeRun()
        {
            
            Console.Clear();
            // Инициализация файловой системы FAT
            fs = new CosmosVFS();
            VFSManager.RegisterVFS(fs);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\r\n  _   _ _______  __\r\n | \\ | | ____\\ \\/ /\r\n |  \\| |  _|  \\  / \r\n | |\\  | |___ /  \\ \r\n |_| \\_|_____/_/\\_\\\r\n                   \r\n");

            

            // Проверка, зарегистрирован ли пользователь
            if (!CheckRegistration())
            {
                RegisterUser();
            }
            else
            {
                AuthenticateUser();
            }

            Console.WriteLine("MyOS is booting...");
            Console.WriteLine("Filesystem initialized (FAT) at " + currentDirectory);
            Console.WriteLine("Type 'Help' for available commands.");

            //МЫшка
            _canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1920, 1080, ColorDepth.ColorDepth32));
            

        }

        // Проверка регистрации
        private bool CheckRegistration()
        {
            if (VFSManager.FileExists("0:\\UserConfig.cfg") &&
                VFSManager.FileExists("0:\\PasswordConfig.cfg"))
            {
                registeredUser = System.IO.File.ReadAllText("0:\\UserConfig.cfg");
                registeredPassword = System.IO.File.ReadAllText("0:\\PasswordConfig.cfg");
                return true;
            }
            return false;
        }

        // Регистрация пользователя
        private void RegisterUser()
        {
            Console.WriteLine("New User Registration");

            Console.Write("Enter your username: ");
            string user = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine("\nRepeat the password: ");
            string retypePassword = "";
            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key != ConsoleKey.Enter)
                {
                    Console.Write("*");
                    retypePassword += keyInfo.KeyChar;
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);

            if (password != retypePassword)
            {
                Console.WriteLine("\nPasswords do not match. Try again.");
                RegisterUser();
                return;
            }

            try
            {
                VFSManager.CreateFile("0:\\UserConfig.cfg");
                VFSManager.CreateFile("0:\\PasswordConfig.cfg");
                System.IO.File.WriteAllText("0:\\UserConfig.cfg", user);
                System.IO.File.WriteAllText("0:\\PasswordConfig.cfg", password);
                Console.WriteLine("\nRegistration completed successfully.");
                registeredUser = user;
                registeredPassword = password;
                isRegistered = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error404: {ex.Message}");
            }
        }

        // Аутентификация пользователя
        private void AuthenticateUser()
        {
            while (true)
            {
                Console.WriteLine("User Login");

                Console.Write("Enter your username: ");
                string user = Console.ReadLine();

                Console.Write("Enter your password: ");
                string password = "";
                ConsoleKeyInfo keyInfo;

                do
                {
                    keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        Console.Write("*");
                        password += keyInfo.KeyChar;
                    }
                }
                while (keyInfo.Key != ConsoleKey.Enter);

                if (user == registeredUser && password == registeredPassword)
                {
                    Console.WriteLine("\nLogin successful.");
                    break; // Выход из цикла, если вход успешен
                }
                else
                {
                    Console.WriteLine("\nInvalid username or password.");
                    Console.WriteLine("1. Try again");
                    Console.WriteLine("2. Register a new account");
                    string choice = Console.ReadLine();

                    if (choice == "2")
                    {
                        RegisterUser();
                        break; // Выход из цикла после регистрации
                    }
                }
            }
        }

        protected override void Run()
        {
           
            _canvas.Clear(Color.Green);
            _canvas.Display();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("NEX>> ");
            Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine();
            Console.WriteLine(input);

            Commands(input);
            Filesystem(input);
            Software(input);
           
        }
        public void Commands(string input)

        {
            switch (input)
            {
                //Вывод списка команд
                case "Help":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("1.Syscom");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("2.Software");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("3.Power");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("4.FyleSystem");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Syscom":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("1.Ver");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("2.Sysinfo");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("3.DateTime");

                    break;
                case "Software":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("1.Calc");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("2.Calendar");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("3.RandomNumber");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "Power":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n1.Shutdown");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("2.Reboot");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                //Отображения версии системы
                case "Ver":
                    Console.Write("Nex Technology [OS version ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("1.0.0");
                    Console.ForegroundColor= ConsoleColor.White;
                    Console.WriteLine("]");
                    break;
                //Отображения информации о железе пользователя
                case "Sysinfo":
                    string CPUbrand = Cosmos.Core.CPU.GetCPUBrandString();
                    string CPUvendor = Cosmos.Core.CPU.GetCPUVendorName();
                    uint amount_of_ram = Cosmos.Core.CPU.GetAmountOfRAM();
                    ulong availible_ram = Cosmos.Core.GCImplementation.GetAvailableRAM();
                    uint UsedRam = Cosmos.Core.GCImplementation.GetUsedRAM();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("CPU: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(CPUbrand);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("CPU Vendor: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(CPUvendor);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("Amount of RAM: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(amount_of_ram);
                    Console.WriteLine(" MB");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("Avialible of RAM: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(availible_ram);
                    Console.WriteLine(" MB");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("Used RAM: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(UsedRam);
                    Console.WriteLine(" B");
                    break;

                //Выключение
                case "Shutdown":
                    Sys.Power.Shutdown();
                    break;
                //Перезагрузка
                case "Reboot":
                    Sys.Power.Reboot();
                    break;
                case "Fylesystem":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Commands:");
                    Console.WriteLine("dir");
                    Console.WriteLine("mkdir <name>");
                    Console.WriteLine("mkfile <name>");
                    Console.WriteLine("delfile <name>");
                    Console.WriteLine("deldir <name>");
                    Console.WriteLine("cd <name>");
                    Console.WriteLine("write <name>");
                    Console.WriteLine("read <name>");
                    Console.WriteLine("free");
                    Console.WriteLine("fstype");
                    break;
                case "DateTime":
                    Console.WriteLine(DateTime.Now);
                    break;

                case "clear":
                    Console.Clear();
                    break;

            }
        }
        //Команды файловой системы
        private void Filesystem(string input)
        {
            string[] parts = input.Split(' ');
            string command = parts[0].ToLower();
            string argument = parts.Length > 1 ? parts[1] : "";

            switch (command)
            {
                // Показать свободное место
                case "free":
                    try
                    {
                        var freeSpace = fs.GetAvailableFreeSpace(currentDirectory);
                        Console.WriteLine("Available Free Space: " + freeSpace + " bytes");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    break;

                // Показать тип файловой системы
                case "fstype":
                    try
                    {
                        var fsType = fs.GetFileSystemType(currentDirectory);
                        Console.WriteLine("File System Type: " + fsType);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    break;

                // Смена директории
                case "cd":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify directory name.");
                    }
                    else
                    {
                        string newDir = currentDirectory + argument;
                        if (VFSManager.DirectoryExists(newDir))
                        {
                            currentDirectory = newDir + @"\";
                            Console.WriteLine("Directory changed to: " + currentDirectory);
                        }
                        else
                        {
                            Console.WriteLine("Error: Directory does not exist.");
                        }
                    }
                    break;

                // Создание файла
                case "mkfile":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify file name.");
                    }
                    else
                    {
                        try
                        {
                            fs.CreateFile(currentDirectory + argument);
                            Console.WriteLine("File created: " + argument);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Создание директории
                case "mkdir":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify directory name.");
                    }
                    else
                    {
                        try
                        {
                            fs.CreateDirectory(currentDirectory + argument);
                            Console.WriteLine("Directory created: " + argument);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Удаление файла
                case "delfile":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify file name.");
                    }
                    else
                    {
                        string filePath = currentDirectory + argument;
                        try
                        {
                            if (VFSManager.FileExists(filePath))
                            {
                                VFSManager.DeleteFile(filePath);
                                Console.WriteLine("File deleted: " + argument);
                            }
                            else
                            {
                                Console.WriteLine("Error: File does not exist.");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Удаление директории
                case "deldir":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify directory name.");
                    }
                    else
                    {
                        string dirPath = currentDirectory + argument;
                        try
                        {
                            if (VFSManager.DirectoryExists(dirPath))
                            {
                                VFSManager.DeleteDirectory(dirPath, true); // Удаляем директорию рекурсивно
                                Console.WriteLine("Directory deleted: " + argument);
                            }
                            else
                            {
                                Console.WriteLine("Error: Directory does not exist.");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Запись в файл
                case "write":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify file name.");
                    }
                    else
                    {
                        string filePath = currentDirectory + argument;
                        Console.WriteLine("Enter text to write:");
                        string text = Console.ReadLine();
                        try
                        {
                            if (!VFSManager.FileExists(filePath))
                            {
                                fs.CreateFile(filePath);
                            }
                            var fileStream = VFSManager.GetFileStream(filePath);
                            byte[] bytes = Encoding.ASCII.GetBytes(text);
                            fileStream.Position = 0; // Записываем с начала файла
                            fileStream.Write(bytes, 0, bytes.Length);
                            Console.WriteLine("Text wrote to " + argument);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Чтение файла
                case "read":
                    if (string.IsNullOrEmpty(argument))
                    {
                        Console.WriteLine("Error: Specify file name.");
                    }
                    else
                    {
                        string filePath = currentDirectory + argument;
                        try
                        {
                            if (VFSManager.FileExists(filePath))
                            {
                                var fileStream = VFSManager.GetFileStream(filePath);
                                byte[] bytes = new byte[fileStream.Length];
                                fileStream.Read(bytes, 0, (int)fileStream.Length);
                                string content = Encoding.ASCII.GetString(bytes);
                                Console.WriteLine("Content of " + argument + ":");
                                Console.WriteLine(content);
                            }
                            else
                            {
                                Console.WriteLine("Error: File does not exist.");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                    break;

                // Вывод списка файлов и директорий
                case "dir":
                    try
                    {
                        var entries = VFSManager.GetDirectoryListing(currentDirectory);
                        if (entries.Count == 0)
                        {
                            Console.WriteLine("Directory is empty.");
                        }
                        foreach (var entry in entries)
                        {
                            if (entry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("| <File>       " + entry.mName);
                            }
                            else if (entry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine("| <Directory>  " + entry.mName);
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                    break;

            }
        }
        public void Software(string input)
        {

            switch (input)
            {
                case "Calc":
                    Calc();
                    break;
                case "Calendar":
                    Calendar();
                    break;
                case "RandomNumber":
                    RandomNumber();
                    break;
            }
        }
        public void Calc()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Choise operation (+, -, *, /, ^, v(for sqrt), b(for break)):");
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            double a = 0, b = 0, c = 0;

            switch (input)
            {
                case '+':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter numb b:");
                    b = double.Parse(Console.ReadLine());
                    c = a + b;
                    Console.WriteLine($"Result: {c}");
                    return;

                case '-':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter numb b:");
                    b = double.Parse(Console.ReadLine());
                    c = a - b;
                    Console.WriteLine($"Result: {c}");
                    return;

                case '*':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter numb b:");
                    b = double.Parse(Console.ReadLine());
                    c = a * b;
                    Console.WriteLine($"Result: {c}");
                    return;

                case '/':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter numb b:");
                    b = double.Parse(Console.ReadLine());
                    if (b != 0)
                    {
                        c = a / b;
                        Console.WriteLine($"Result: {c}");
                    }
                    else
                    {
                        Console.WriteLine("Error:  you cannot divide by zero.");
                    }
                    return;

                case '^':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    Console.WriteLine("Enter power b:");
                    b = double.Parse(Console.ReadLine());
                    c = Math.Pow(a, b);
                    Console.WriteLine($"Result: {c}");
                    return;

                case 'v':
                    Console.WriteLine("Enter numb a:");
                    a = double.Parse(Console.ReadLine());
                    c = Math.Sqrt(a);
                    Console.WriteLine($"Result: {c}");
                    return;

                case 'b':
                    break;
            }
        }
        public void Calendar()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Calendar");
            Console.WriteLine("=========");
            Console.ForegroundColor = ConsoleColor.White;

            // Массивы месяцев и дней
            string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // Получение текущей даты из RTC
            int year = RTC.Year;
            int month = RTC.Month;
            int day = RTC.DayOfTheWeek;

            // Проверка високосного года
            if (IsLeapYear(year))
            {
                daysInMonth[1] = 29;
            }

            // Вывод названия месяца и года
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{monthNames[month - 1]} {year}");
            Console.WriteLine("Mon Tue Wed Thu Fri Sat Sun");
            Console.ForegroundColor = ConsoleColor.White;

            // Расчет первого дня месяца
            int firstDayOfMonth = CalculateDayOfWeek(1, month, year);
            int startDay = (firstDayOfMonth + 6) % 7 + 1;

            // Вывод пробелов до первого дня
            for (int i = 1; i < startDay; i++)
            {
                Console.Write("    ");
            }

            // Вывод дней месяца
            int daysInCurrentMonth = daysInMonth[month - 1];
            for (int currentDay = 1; currentDay <= daysInCurrentMonth; currentDay++)
            {
                // Подсвечиваем текущий день
                if (currentDay == day)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"{currentDay,2} ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write($"{currentDay,2} ");
                }

                // Переход на новую строку после воскресенья
                if ((startDay + currentDay - 1) % 7 == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        // Вспомогательные методы
        private bool IsLeapYear(int year)
        {
            return year % 4 == 0 && year % 100 != 0 || year % 400 == 0;
        }

        private int CalculateDayOfWeek(int day, int month, int year)
        {
            if (month < 3)
            {
                month += 12;
                year -= 1;
            }
            int K = year % 100;
            int J = year / 100;
            int h = (day + 13 * (month + 1) / 5 + K + K / 4 + J / 4 + 5 * J) % 7;
            return h;
        }
        public void RandomNumber()
        {
            Random random = new Random();
            int secretNumber = random.Next(1, 101); // Случайное число от 1 до 100
            int attempts = 0;
            int guess = 0;

            Console.WriteLine("Welcome to the game guess the number'!");
            Console.WriteLine("I made a number from 1 to 100. Try to guess!");

            while (guess != secretNumber)
            {
                Console.Write("Enter your guess: ");

                if (!int.TryParse(Console.ReadLine(), out guess))
                {
                    Console.WriteLine("Please enter an integer!");
                    continue;
                }

                attempts++;

                if (guess > secretNumber)
                {
                    Console.WriteLine("Too little! Try again.");
                }
                else if (guess < secretNumber)
                {
                    Console.WriteLine("Too many! Try again.");
                }
            }

            Console.WriteLine($"Congratulations! You guessed the number  {secretNumber} for {attempts} attempts");
            Console.WriteLine("Press any button...");
            Console.ReadKey();
        }
    }
}