using System.Text.Json;

namespace Radio
{
    internal interface IStations
    {
        Station? Choose(string lastUrl);
    }

    internal class Stations(string stationsFilename) : IStations
    {
        private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

        private static readonly Station[] _defaultStations = [
            new Station { Name = "Secret Agent", Url = "https://somafm.com/secretagent130.pls" },
        ];

        private static void SetCursorPosition(int pos)
        {
            Console.Write("\r \r");
            Console.SetCursorPosition(0, pos);
            Console.Write('>');
        }

        public Station? Choose(string lastUrl)
        {
            if (!File.Exists(stationsFilename))
            {
                Console.WriteLine($"Initializing {stationsFilename}");

                File.WriteAllText(stationsFilename, JsonSerializer.Serialize(_defaultStations, _serializerOptions));
            }

            var text = File.ReadAllText(stationsFilename);
            var stations = JsonSerializer.Deserialize<Station[]>(text) ?? throw new Exception("Could not deserialize stations");
            var lastIndex = 0;

            Console.WriteLine("Choose a station:");

            foreach (var station in stations.Select((value, index) => (value, index)))
            {
                Console.WriteLine($" {station.value.Name}");

                if (station.value.Url == lastUrl)
                {
                    lastIndex = station.index;
                }
            }

            var cursorMax = Console.CursorTop - 1;
            var cursorMin = cursorMax - stations.Length + 1;

            Console.SetCursorPosition(0, cursorMin + lastIndex);
            Console.Write('>');

            var key = Console.ReadKey(true).Key;

            while (key != ConsoleKey.Enter && key != ConsoleKey.Q)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (Console.CursorTop > cursorMin)
                        {
                            SetCursorPosition(Console.CursorTop - 1);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (Console.CursorTop < cursorMax)
                        {
                            SetCursorPosition(Console.CursorTop + 1);
                        }
                        break;
                    case ConsoleKey.PageUp:
                        if (Console.CursorTop > cursorMin)
                        {
                            SetCursorPosition(Math.Max(Console.CursorTop - 10, cursorMin));
                        }
                        break;
                    case ConsoleKey.PageDown:
                        if (Console.CursorTop < cursorMax)
                        {
                            SetCursorPosition(Math.Min(Console.CursorTop + 10, cursorMax));
                        }
                        break;
                    case ConsoleKey.Home:
                        if (Console.CursorTop > cursorMin)
                        {
                            SetCursorPosition(cursorMin);
                        }
                        break;
                    case ConsoleKey.End:
                        if (Console.CursorTop < cursorMax)
                        {
                            SetCursorPosition(cursorMax);
                        }
                        break;
                }

                key = Console.ReadKey(true).Key;
            }

            var index = Console.CursorTop - cursorMin;

            Console.SetCursorPosition(0, cursorMin - 1);

            var spaces = new string(' ', Console.WindowWidth - 1);

            for (int i = cursorMin - 1; i <= cursorMax; ++i)
            {
                Console.WriteLine(spaces);
            }

            Console.SetCursorPosition(0, cursorMin - 1);

            return key == ConsoleKey.Q ? null : stations[index];
        }
    }
}
