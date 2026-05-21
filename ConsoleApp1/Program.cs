using System;
using System.Collections.Generic; 

namespace Практична_робота__9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Перевірка патерна SINGLETON:");

            Logger.GetInstance().AddLog("Програма успішно запустилася.");
            Logger.GetInstance().AddLog("Користувач зайшов у меню.");
            Logger.GetInstance().ShowLogs();

            Console.WriteLine();

            Settings appSettings = Settings.GetInstance();
            Console.WriteLine($"Поточна мова: {appSettings.Language}");
            Console.WriteLine($"Розмір вікна: {appSettings.WindowSize}");

            appSettings.Language = "EN";

            Console.WriteLine($"Оновлена мова (через новий виклик): {Settings.GetInstance().Language}");

            Console.WriteLine("\n--------------------------------------------\n");

            Console.WriteLine("Перевірка патерна BUILDER:");

            GameDirector director = new GameDirector();
            WarriorBuilder warriorBuilder = new WarriorBuilder();

            Character myWarrior = director.Construct(warriorBuilder);
            Console.WriteLine(myWarrior.ToString());

            Console.WriteLine();

            string mySqlQuery = new SqlBuilder()
                .Select("id, username, email")
                .Where("is_active = 1")
                .OrderBy("username DESC")
                .GetResult();

            Console.WriteLine("Згенерований SQL-запит:");
            Console.WriteLine(mySqlQuery);

            Console.WriteLine("\n--------------------------------------------\n");

            Console.WriteLine("Перевірка патерна BRIDGE:");

            Console.WriteLine("--- 1. Система повідомлень ---");
            IMessageSender emailSender = new EmailSender();
            IMessageSender smsSender = new SmsSender();

            Message textMessage = new TextMessage(emailSender);
            textMessage.Send("Привіт! Це звичайне робоче повідомлення.");

            Message htmlMessage = new HtmlMessage(smsSender);
            htmlMessage.Send("<h1>Увага!</h1><p>Акцію активовано.</p>");

            Console.WriteLine();

            Console.WriteLine("--- 2. Керування пристроями ---");
            IDevice tv = new TV();
            IDevice radio = new Radio();

            RemoteControl basicRemote = new RemoteControl(tv);
            Console.WriteLine("Працюємо із звичайним пультом (ТБ):");
            basicRemote.Power();
            basicRemote.VolumeUp();

            Console.WriteLine();

            AdvancedRemoteControl advancedRemote = new AdvancedRemoteControl(radio);
            Console.WriteLine("Працюємо з просунутим пультом (Радіо):");
            advancedRemote.Power();
            advancedRemote.Mute();

            Console.ReadKey();
        }
    }

    // ПАТЕРН: SINGLETON (ОДИНАК)

    public class Logger
    {
        private static Logger _instance;
        private List<string> _logs = new List<string>();

        private Logger() { }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Logger();
            }
            return _instance;
        }

        public void AddLog(string message)
        {
            _logs.Add($"{DateTime.Now}: {message}");
        }

        public void ShowLogs()
        {
            Console.WriteLine("--- Журнал подій ---");
            foreach (var log in _logs)
            {
                Console.WriteLine(log);
            }
        }
    }

    public class Settings
    {
        private static Settings _instance;

        public string Language { get; set; } = "UA";
        public string WindowSize { get; set; } = "1920x1080";

        private Settings() { }

        public static Settings GetInstance()
        {
            if (_instance == null)
                _instance = new Settings();
            return _instance;
        }
    }

    // ПАТЕРН: BUILDER (БУДІВНИК ПЕРСОНАЖА)

    public class Character
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        public override string ToString() =>
            $"Персонаж: Сила={Strength}, Спритність={Agility}, Інтелект={Intelligence}";
    }

    public interface ICharacterBuilder
    {
        void SetStrength();
        void SetAgility();
        void SetIntelligence();
        Character GetCharacter();
    }

    public class WarriorBuilder : ICharacterBuilder
    {
        private Character _character = new Character();
        public void SetStrength() => _character.Strength = 100;
        public void SetAgility() => _character.Agility = 50;
        public void SetIntelligence() => _character.Intelligence = 20;
        public Character GetCharacter() => _character;
    }

    public class GameDirector
    {
        public Character Construct(ICharacterBuilder builder)
        {
            builder.SetStrength();
            builder.SetAgility();
            builder.SetIntelligence();
            return builder.GetCharacter();
        }
    }

    // ПАТЕРН: BUILDER (ГЕНЕРАТОР SQL ЗАПИТІВ)

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

    // ПАТЕРН: BRIDGE (МІСТ)
    public interface IMessageSender
    {
        void SendMessage(string formattedBody);
    }

    public class EmailSender : IMessageSender
    {
        public void SendMessage(string formattedBody) =>
            Console.WriteLine($"[Email] Відправлено на пошту: {formattedBody}");
    }

    public class SmsSender : IMessageSender
    {
        public void SendMessage(string formattedBody) =>
            Console.WriteLine($"[SMS] Відправлено через мобільного оператора: {formattedBody}");
    }

    public abstract class Message
    {
        protected IMessageSender sender;

        public Message(IMessageSender sender)
        {
            this.sender = sender;
        }

        public abstract void Send(string text);
    }

    public class TextMessage : Message
    {
        public TextMessage(IMessageSender sender) : base(sender) { }

        public override void Send(string text)
        {
            sender.SendMessage($"[TEXT] {text}");
        }
    }

    public class HtmlMessage : Message
    {
        public HtmlMessage(IMessageSender sender) : base(sender) { }

        public override void Send(string text)
        {
            sender.SendMessage($"[HTML] <html><body>{text}</body></html>");
        }
    }


    public interface IDevice
    {
        void TurnOn();
        void TurnOff();
        void SetVolume(int percent);
    }

    public class TV : IDevice
    {
        public void TurnOn() => Console.WriteLine("Телевізор -> УВІМКНЕНО");
        public void TurnOff() => Console.WriteLine("Телевізор -> ВИМКНЕНО");
        public void SetVolume(int percent) => Console.WriteLine($"Телевізор -> Гучність встановлено на {percent}%");
    }

    public class Radio : IDevice
    {
        public void TurnOn() => Console.WriteLine("Радіоприймач -> УВІМКНЕНО");
        public void TurnOff() => Console.WriteLine("Радіоприймач -> ВИМКНЕНО");
        public void SetVolume(int percent) => Console.WriteLine($"Радіоприймач -> Гучність встановлено на {percent}%");
    }

    public class RemoteControl
    {
        protected IDevice device;
        protected bool isPowered = false;

        public RemoteControl(IDevice device)
        {
            this.device = device;
        }

        public virtual void Power()
        {
            if (isPowered)
            {
                device.TurnOff();
                isPowered = false;
            }
            else
            {
                device.TurnOn();
                isPowered = true;
            }
        }

        public virtual void VolumeUp() => device.SetVolume(30);
    }

    public class AdvancedRemoteControl : RemoteControl
    {
        public AdvancedRemoteControl(IDevice device) : base(device) { }

        public void Mute()
        {
            Console.WriteLine("Пульт -> Натиснуто кнопку швидкого вимкнення звуку (Mute)");
            device.SetVolume(0);
        }
    }
}