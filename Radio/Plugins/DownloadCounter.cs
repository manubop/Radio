using Un4seen.Bass;

namespace Radio.Plugins
{
    internal class DownloadCounter(int mod) : IUIPlugin
    {
        private long _counter = 0;
        private string _text = "Download: ...";

        /// <summary>
        /// See <see cref="https://pingfu.net/convert-bytes-to-kb-mb-gb-tb-or-higher">original implementation</see>
        /// </summary>
        /// <param name="bytes">Size in bytes</param>
        /// <returns>Formatted string</returns>
        private static string FormatBytes(long bytes)
        {
            if (bytes >= 0x40000000)
            {
                return ((double)(bytes >> 20) / 1024).ToString("0.00 GB");
            }

            if (bytes >= 0x100000)
            {
                return ((double)(bytes >> 10) / 1024).ToString("0.00 MB");
            }

            return ((double)bytes / 1024).ToString("0.00 KB");
        }

        public int Update(IStream stream)
        {
            if (_counter++ % mod == 0)
            {
                var pos = stream.GetFilePosition(BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD);

                _text = $"Download: {FormatBytes(pos),-10}";
            }

            Console.WriteLine(_text);

            return 1;
        }

        public int Clear()
        {
            Console.WriteLine(new string(' ', _text.Length));

            return 1;
        }

        public void Reset()
        {
            _counter = 0;
            _text = "Download: ...";
        }
    }
}
