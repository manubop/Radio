using System.Text;
using Un4seen.Bass;

namespace Radio.Plugins
{
    internal class VUMeterPlugin(int max) : IUIPlugin
    {
        private readonly List<string> _vuMeters = new(EnumerateVUMeters(max));
        private readonly StringBuilder _sb = new(max + 2);
        private readonly string _empty = new(' ', max + 2);

        private string? _msg;
        private int _duration;

        private static IEnumerable<string> EnumerateVUMeters(int max)
        {
            for (int i = 0; i <= max; ++i)
            {
                yield return '[' + new string('=', i).PadRight(max) + ']';
            }
        }

        private static int MinMaxValue(int v, int min, int max) => v < min ? min : v > max ? max : v;

        public int Update(IStream stream)
        {
            var levels = stream.GetLevel();
            var left = MinMaxValue(Utils.LowWord32(levels) * max / short.MaxValue, 0, max);
            var right = MinMaxValue(Utils.HighWord32(levels) * max / short.MaxValue, 0, max);

            if (_msg == null)
            {
                Console.WriteLine(_vuMeters[left]);
            }
            else
            {
                _sb.Clear();
                _sb.Append(_vuMeters[left]);

                var offset = (_sb.Length - _msg.Length) / 2;

                for (int i = 0; i < _msg.Length; ++i)
                {
                    _sb[i + offset] = _msg[i];
                }

                Console.WriteLine(_sb);

                if (--_duration <= 0)
                {
                    _msg = null;
                }
            }

            Console.WriteLine(_vuMeters[right]);

            return 2;
        }

        public int Clear()
        {
            Console.WriteLine(_empty);
            Console.WriteLine(_empty);

            return 2;
        }

        public void Reset()
        {
            _msg = null;
        }

        public void SetMessage(string msg, int duration)
        {
            _msg = ' ' + msg + ' ';
            _duration = duration;
        }
    }
}
