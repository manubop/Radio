using System.Text.Json;

namespace Radio
{
    internal interface IOptions
    {
        public bool ShowDownloadInfo { get; }
        public bool ShowICYTags { get; }
        public int VUMetersLength { get; }
        public bool ShowBitRate { get; }
    }

    internal class Options : IOptions
    {
        public bool ShowDownloadInfo { get; set; } = false;
        public bool ShowICYTags { get; set; } = false;
        public int VUMetersLength { get; set; } = 80;
        public bool ShowBitRate { get; set; } = false;

        public static IOptions ReadFrom(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Initializing default options file: {filename}");

                JsonSerializer.Serialize(new Options());
            }

            var text = File.ReadAllText(filename);
            var options = JsonSerializer.Deserialize<Options>(text);

            if (options == null)
            {
                Console.WriteLine($"Failed to read options file: {filename}");

                return new Options();
            }

            return options;
        }
    }
}
