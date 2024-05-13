// See https://aka.ms/new-console-template for more information
using Radio;
using Radio.Plugins;
using Un4seen.Bass;

var GetPlugins = (IOptions options) =>
{
    var plugins = new UIPluginStack();

    plugins.Add(new VUMeterPlugin(options.VUMetersLength));

    if (options.ShowBitRate)
    {
        plugins.Add(new BitrateCounter(40));
    }

    plugins.Add(new DownloadCounter(20));
    plugins.Add(new TimeCounter(20));

    return plugins;
};

if (Utils.HighWord(Bass.BASS_GetVersion()) != Bass.BASSVERSION)
{
    Console.WriteLine("Wrong Bass Version!");
    Environment.Exit(1);
}

Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);

if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, -1))
{
    Console.WriteLine("Could not initialize BASS");
    Environment.Exit(1);
}

Console.CursorVisible = false;

var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var stationsFilename = Path.Combine(homeDir, ".radio-console", "stations.json");
var optionsFilename = Path.Combine(homeDir, ".radio-console", "options.json");
var stations = new Stations(stationsFilename);
var options = Options.ReadFrom(optionsFilename);
var plugins = GetPlugins(options);
var ui = new UI(stations, options, plugins);

await ui.Main(args, 50);

Console.CursorVisible = true;

Bass.BASS_Stop();
Bass.BASS_Free();
