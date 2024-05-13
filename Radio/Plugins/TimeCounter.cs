namespace Radio.Plugins
{
    internal class TimeCounter(int mod) : IUIPlugin
    {
        private DateTime _start = DateTime.Now;
        private string _text = "Time: ...";
        private int _counter = 0;

        public int Clear()
        {
            Console.WriteLine(new string(' ', _text.Length));

            return 1;
        }

        public void Reset()
        {
            _start = DateTime.Now;
            _text = "Time: ...";
            _counter = 0;
        }

        public int Update(IStream stream)
        {
            if (_counter++ % mod == 0)
            {
                var diff = DateTime.Now - _start;

                _text = $"Time: {diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
            }

            Console.WriteLine(_text);

            return 1;
        }
    }
}
