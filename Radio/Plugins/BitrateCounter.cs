using Un4seen.Bass;

namespace Radio.Plugins
{
    internal class BitrateCounter(int mod) : IUIPlugin
    {
        private long _prevPos = 0;
        private DateTime _start = DateTime.Now;
        private long _counter = 0;
        private string _text = "Bitrate: ...";

        public int Update(IStream stream)
        {
            _counter = ++_counter % mod;

            if (_counter == 0)
            {
                var pos = stream.GetFilePosition(BASSStreamFilePosition.BASS_FILEPOS_CURRENT);
                var bits = (pos - _prevPos) * 8;
                var stop = DateTime.Now;
                var sec = (stop - _start).TotalSeconds;

                _text = $"Bitrate: {bits / sec / 1000,4:0}kbps";

                _prevPos = pos;
                _start = stop;
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
            _prevPos = 0;
            _start = DateTime.Now;
            _counter = 0;
            _text = "Bitrate: ...";
        }
    }
}
