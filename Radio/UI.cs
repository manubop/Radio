using System.Runtime.InteropServices;
using Radio.Plugins;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace Radio
{
    internal enum STATUS
    {
        CONTINUE,
        QUIT,
        CHANGE,
    }

    internal class Loop(IStream stream, IUIPlugin plugin)
    {
        private void SetVolume(float vol)
        {
            stream.SetAttribute(BASSAttribute.BASS_ATTRIB_VOL, vol);
            plugin.SetMessage($"Vol: {Math.Round(vol * 100),-3}%", 40);
        }

        public async Task<STATUS> Do(int delay)
        {
            var status = STATUS.CONTINUE;

            stream.SetSyncProc(BASSSync.BASS_SYNC_END, (int handle, int channel, int data, IntPtr user) =>
            {
                status = STATUS.CHANGE;
            });

            while (status == STATUS.CONTINUE)
            {
                if (!Console.KeyAvailable)
                {
                    plugin.Update(stream);

                    await Task.Delay(delay);

                    continue;
                }

                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Add:
                        {
                            float vol = 0f;

                            if (!stream.GetAttribute(BASSAttribute.BASS_ATTRIB_VOL, ref vol))
                            {
                                Console.WriteLine("Could not get channel volume attribute");
                                break;
                            }

                            if (vol < 1f)
                            {
                                vol += 0.1f;

                                if (vol > 1f)
                                {
                                    vol = 1f;
                                }

                                SetVolume(vol);
                            }

                            break;
                        }

                    case ConsoleKey.Subtract:
                        {
                            float vol = 0f;

                            if (!stream.GetAttribute(BASSAttribute.BASS_ATTRIB_VOL, ref vol))
                            {
                                Console.WriteLine("Could not get channel volume attribute");
                                break;
                            }

                            if (vol > 0f)
                            {
                                vol -= 0.1f;

                                if (vol < 0f)
                                {
                                    vol = 0f;
                                }

                                SetVolume(vol);
                            }

                            break;
                        }

                    case ConsoleKey.C:
                        status = STATUS.CHANGE;
                        break;

                    case ConsoleKey.Q:
                        status = STATUS.QUIT;
                        break;

                    default:
                        Console.WriteLine($"Unhandled key: {keyInfo.Key}");
                        break;
                }
            }

            plugin.Clear();

            return status;
        }
    }

    internal class UI(IStations stations, IOptions options, IUIPlugin plugin)
    {
        private IStream? CreateStream(string url)
        {
            Console.WriteLine($"Opening {url}");

            var stream = Stream.Create(url, BASSFlag.BASS_STREAM_STATUS, (IntPtr buffer, int length, IntPtr user) =>
            {
                if (buffer != IntPtr.Zero && length == 0 && options.ShowDownloadInfo)
                {
                    var txt = Marshal.PtrToStringAnsi(buffer);

                    Console.WriteLine(txt);
                }
            });

            if (stream == null)
            {
                Console.WriteLine($"Could not create stream from {url}");
                return null;
            }

            var channelInfo = stream.GetInfo();

            Console.WriteLine($"Channel info: {channelInfo}");

            if (channelInfo.ctype == BASSChannelType.BASS_CTYPE_STREAM_MF)
            {
                var wftext = stream.GetTagsWAVEFORMAT();

                if (wftext != null)
                {
                    Console.WriteLine($"Sample rate: {wftext.waveformatex.nAvgBytesPerSec * 8 / 1000}kbps");
                }
            }

            if (options.ShowICYTags)
            {
                var icy = stream.GetTagsICY() ?? stream.GetTagsHTTP() ?? [];

                foreach (var tag in icy)
                {
                    Console.WriteLine($"ICY: {tag}");
                }
            }

            return stream;
        }

        private static void SetupTagDisplay(IStream stream, string url, Action clear)
        {
            var tagInfo = new TAG_INFO(url);

            if (stream.GetTagInfo(tagInfo))
            {
                tagInfo.Display();
            }

            var displayedTagInfo = new TagInfo(tagInfo);

            stream.SetSyncProc(BASSSync.BASS_SYNC_META, (int handle, int channel, int data, IntPtr user) =>
            {
                var tags = Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META);

                if (tagInfo.UpdateFromMETA(tags, true, true) && displayedTagInfo.Update(tagInfo))
                {
                    clear();
                    tagInfo.Display();
                }
            });
        }

        public async Task Main(IEnumerable<string> urls, int period)
        {
            var status = STATUS.CONTINUE;
            var url = string.Empty;

            while (status != STATUS.QUIT)
            {
                if (urls.Any())
                {
                    url = urls.First();
                    urls = urls.Skip(1);
                }
                else
                {
                    var station = stations.Choose(url);

                    if (station == null)
                    {
                        break;
                    }

                    url = station.Url;
                }

                using var stream = CreateStream(url);

                if (stream != null)
                {
                    plugin.Reset();

                    SetupTagDisplay(stream, url, () => plugin.Clear());

                    if (!stream.Play(false))
                    {
                        Console.WriteLine("Failed to start stream");
                    }

                    var loop = new Loop(stream, plugin);

                    status = await loop.Do(period);
                }
            }
        }
    }
}
