namespace Radio.Plugins
{
    internal interface IUIPlugin
    {
        int Update(IStream stream);
        int Clear();
        void Reset();
        void SetMessage(string msg, int duration)
        {
        }
    }

    internal class UIPluginStack : IUIPlugin
    {
        private readonly List<IUIPlugin> _plugins = [];

        public void Add(IUIPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public int Update(IStream stream)
        {
            var count = _plugins.Aggregate(0, (acc, plugin) => acc += plugin.Update(stream));

            Console.SetCursorPosition(0, Console.CursorTop - count);

            return count;
        }

        public int Clear()
        {
            var count = _plugins.Aggregate(0, (acc, plugin) => acc += plugin.Clear());

            Console.SetCursorPosition(0, Console.CursorTop - count);

            return count;
        }

        public void Reset()
        {
            foreach (var plugin in _plugins)
            {
                plugin.Reset();
            }
        }

        public void SetMessage(string msg, int duration)
        {
            foreach (var plugin in _plugins)
            {
                plugin.SetMessage(msg, duration);
            }
        }
    }
}
