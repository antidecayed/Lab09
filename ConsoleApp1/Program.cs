using System;
using System.Collections.Generic;

namespace Практична_робота__9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("    Головне меню: ОБЕРІТЬ ПАТЕРН            ");
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("1. Перевірка патерна SINGLETON (Одинак)");
                Console.WriteLine("2. Перевірка патерна BUILDER (Будівник)");
                Console.WriteLine("3. Перевірка патерна BRIDGE (Міст)");
                Console.WriteLine("4. Вихід з програми");
                Console.WriteLine("--------------------------------------------");
                Console.Write("Ваш вибір (1-4): ");

                string mainMenuChoice = Console.ReadLine();

                switch (mainMenuChoice)
                {
                    case "1":
                        RunSingletonDemo();
                        break;
                    case "2":
                        RunBuilderDemo();
                        break;
                    case "3":
                        RunBridgeDemo();
                        break;
                    case "4":
                        Console.WriteLine("\nДякую за використання програми! До побачення.");
                        return;
                    default:
                        Console.WriteLine("\nНекоректний вибір! Натисніть будь-яку клавішу, щоб спробувати знову.");
                        Console.ReadKey();
                        continue;
                }

                Console.WriteLine("\n--------------------------------------------");
                Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися до Головного Меню.");
                Console.ReadKey();
            }
        }

        private static void RunSingletonDemo()
        {
            Console.Clear();
            Console.WriteLine("ПЕРЕВІРКА ПАТЕРНА SINGLETON:");

            Console.WriteLine("Виберіть поточну мову інтерфейсу:");
            Console.WriteLine("1 - Українська");
            Console.WriteLine("2 - Англійська");
            Console.Write("Ваш вибір: ");
            string langInput = Console.ReadLine();
            string selectedLanguage = (langInput == "2") ? "Англійська" : "Українська";

            Console.Write("Введіть розмір вікна (наприклад, 1920x1080 або 1024x768): ");
            string selectedSize = Console.ReadLine();

            Settings appSettings = Settings.GetInstance();
            appSettings.Language = selectedLanguage;
            appSettings.WindowSize = selectedSize;

            Logger.GetInstance().AddLog($"Користувач змінив налаштування. Мова: {selectedLanguage}, Розмір: {selectedSize}");

            Console.WriteLine("\n[Результат] Дані, які збереглися v єдиному екземплярі Settings:");
            Console.WriteLine($"Поточна мова в системі: {Settings.GetInstance().Language}");
            Console.WriteLine($"Розмір вікна в системі: {Settings.GetInstance().WindowSize}");

            Console.WriteLine();
            Logger.GetInstance().ShowLogs();
        }

        private static void RunBuilderDemo()
        {
            Console.Clear();
            Console.WriteLine("ПЕРЕВІРКА ПАТЕРНА BUILDER:");

            Console.Write("Введіть ім'я вашого персонажа: ");
            string charName = Console.ReadLine();

            Console.Write("Введіть показник Сили (число): ");
            int.TryParse(Console.ReadLine(), out int strength);

            Console.Write("Введіть показник Спритності (число): ");
            int.TryParse(Console.ReadLine(), out int agility);

            Console.Write("Введіть показник Інтелекту (число): ");
            int.TryParse(Console.ReadLine(), out int intelligence);

            GameDirector director = new GameDirector();
            CustomCharacterBuilder customBuilder = new CustomCharacterBuilder();

            Character userCharacter = director.Construct(customBuilder, charName, strength, agility, intelligence);

            Console.WriteLine("\nПерсонаж успішно створений через патерн Будівник:");
            Console.WriteLine(userCharacter.ToString());

            Console.WriteLine("\n--- Автоматичний тест SQL-Будівника ---");
            string generatedQuery = new SqlBuilder()
                .Select("id, name, score")
                .Where("score > 50")
                .OrderBy("name ASC")
                .GetResult();
            Console.WriteLine($"Згенерований SQL-запит: {generatedQuery}");
        }

        private static void RunBridgeDemo()
        {
            Console.Clear();
            Console.WriteLine("ПЕРЕВІРКА ПАТЕРНА BRIDGE:");
            Console.WriteLine("1. Системні повідомлення (Email / SMS)");
            Console.WriteLine("2. Керування пристроями (Телевізор / Радіо)");
            Console.Write("Ваш вибір: ");
            string bridgeChoice = Console.ReadLine();

            if (bridgeChoice == "2")
            {
                Console.Clear();
                Console.WriteLine("--- Демонстрація керування пристроями через Міст ---");

                IDevice tv = new TV();
                RemoteControl remote = new BasicRemote(tv);

                tv.PrintStatus();
                remote.TogglePower();
                remote.VolumeUp();
                tv.PrintStatus();

                Console.WriteLine("\nПеремикаємо цей же пульт на інший пристрій (Радіо):");
                IDevice radio = new Radio();
                RemoteControl radioRemote = new BasicRemote(radio);

                radio.PrintStatus();
                radioRemote.TogglePower();
                radioRemote.VolumeDown();
                radio.PrintStatus();
            }
            else
            {
                Console.Clear();
                Console.Write("Введіть текст повідомлення, яке надійде на Пошту: ");
                string emailBody = Console.ReadLine();

                Console.Write("Введіть текст повідомлення, яке надійде через SMS: ");
                string smsBody = Console.ReadLine();

                IMessageSender emailSender = new EmailSender();
                IMessageSender smsSender = new SmsSender();

                Console.WriteLine("\nВідправка через Міст (Bridge):");

                Message htmlEmail = new HtmlMessage(emailSender);
                htmlEmail.Send(emailBody);

                Message textSms = new TextMessage(smsSender);
                textSms.Send(smsBody);
            }
        }
    }

    public class Logger
    {
        private static Logger _instance;
        private List<string> _logs = new List<string>();

        private Logger() { }

        public static Logger GetInstance()
        {
            if (_instance == null) _instance = new Logger();
            return _instance;
        }

        public void AddLog(string message)
        {
            _logs.Add($"{DateTime.Now}: {message}");
        }

        public void ShowLogs()
        {
            Console.WriteLine("--- Журнал подій (Logger) ---");
            foreach (var log in _logs) Console.WriteLine(log);
        }
    }

    public class Settings
    {
        private static Settings _instance;
        public string Language { get; set; } = "Українська";
        public string WindowSize { get; set; } = "1920x1080";

        private Settings() { }

        public static Settings GetInstance()
        {
            if (_instance == null) _instance = new Settings();
            return _instance;
        }
    }

    public class Character
    {
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        public override string ToString() =>
            $"Ім'я: {Name} | Характеристики: Сила = {Strength}, Спритність = {Agility}, Інтелект = {Intelligence}";
    }

    public interface ICharacterBuilder
    {
        void SetName(string name);
        void SetStrength(int strength);
        void SetAgility(int agility);
        void SetIntelligence(int intelligence);
        Character GetCharacter();
    }

    public class CustomCharacterBuilder : ICharacterBuilder
    {
        private Character _character = new Character();
        public void SetName(string name) => _character.Name = name;
        public void SetStrength(int strength) => _character.Strength = strength;
        public void SetAgility(int agility) => _character.Agility = agility;
        public void SetIntelligence(int intelligence) => _character.Intelligence = intelligence;
        public Character GetCharacter() => _character;
    }

    public class GameDirector
    {
        public Character Construct(ICharacterBuilder builder, string name, int str, int agl, int intel)
        {
            builder.SetName(name);
            builder.SetStrength(str);
            builder.SetAgility(agl);
            builder.SetIntelligence(intel);
            return builder.GetCharacter();
        }
    }

    public class SqlQuery
    {
        public string SelectPart { get; set; }
        public string WherePart { get; set; }
        public string OrderByPart { get; set; }

        public string Build() =>
            $"SELECT {SelectPart} FROM MyTable" +
            (string.IsNullOrEmpty(WherePart) ? "" : $" WHERE {WherePart}") +
            (string.IsNullOrEmpty(OrderByPart) ? "" : $" ORDER BY {OrderByPart}");
    }

    public class SqlBuilder
    {
        private SqlQuery _query = new SqlQuery();
        public SqlBuilder Select(string columns) { _query.SelectPart = columns; return this; }
        public SqlBuilder Where(string condition) { _query.WherePart = condition; return this; }
        public SqlBuilder OrderBy(string order) { _query.OrderByPart = order; return this; }
        public string GetResult() => _query.Build();
    }

    public interface IMessageSender
    {
        void SendMessage(string formattedBody);
    }

    public class EmailSender : IMessageSender
    {
        public void SendMessage(string formattedBody) =>
            Console.WriteLine($"Лист відправлено на електронну пошту: {formattedBody}");
    }

    public class SmsSender : IMessageSender
    {
        public void SendMessage(string formattedBody) =>
            Console.WriteLine($"Повідомлення доставлено на мобільний телефон через SMS: {formattedBody}");
    }

    public abstract class Message
    {
        protected IMessageSender sender;
        public Message(IMessageSender sender) => this.sender = sender;
        public abstract void Send(string text);
    }

    public class TextMessage : Message
    {
        public TextMessage(IMessageSender sender) : base(sender) { }
        public override void Send(string text) => sender.SendMessage($"{text}");
    }

    public class HtmlMessage : Message
    {
        public HtmlMessage(IMessageSender sender) : base(sender) { }

        public override void Send(string text) =>
            sender.SendMessage($"[HTML-Формат]: <html><body>{text}</body></html>");
    }

    public interface IDevice
    {
        bool IsPoweredOn { get; set; }
        int Volume { get; set; }
        void PrintStatus();
    }

    public class TV : IDevice
    {
        public bool IsPoweredOn { get; set; } = false;
        public int Volume { get; set; } = 20;
        public void PrintStatus() =>
            Console.WriteLine($"[Пристрій: Телевізор] Стан: {(IsPoweredOn ? "Увімкнено" : "Вимкнено")}, Гучність: {Volume}%");
    }

    public class Radio : IDevice
    {
        public bool IsPoweredOn { get; set; } = false;
        public int Volume { get; set; } = 10;
        public void PrintStatus() =>
            Console.WriteLine($"[Пристрій: Радіоприймач] Стан: {(IsPoweredOn ? "Увімкнено" : "Вимкнено")}, Гучність: {Volume}%");
    }

    public abstract class RemoteControl
    {
        protected IDevice device;
        public RemoteControl(IDevice device) => this.device = device;

        public void TogglePower()
        {
            device.IsPoweredOn = !device.IsPoweredOn;
            Console.WriteLine("Пульт: змінено стан живлення пристрою.");
        }

        public void VolumeUp()
        {
            if (device.Volume < 100) device.Volume += 10;
            Console.WriteLine("Пульт: гучність збільшено на +10.");
        }

        public void VolumeDown()
        {
            if (device.Volume > 0) device.Volume -= 10;
            Console.WriteLine("Пульт: гучність зменшено на -10.");
        }
    }

    public class BasicRemote : RemoteControl
    {
        public BasicRemote(IDevice device) : base(device) { }
    }
}