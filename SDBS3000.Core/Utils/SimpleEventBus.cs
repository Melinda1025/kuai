using System.Collections.Concurrent;

namespace SDBS3000.Core.Utils
{
    public class SimpleEventBus
    {
        private readonly ConcurrentDictionary<string, List<EventHandler>> _eventDict = new();

        private SimpleEventBus() { }
        public static SimpleEventBus Instance { get; private set; } = new();

        public void Publish(string key, object sender)
        {
            if (_eventDict.TryGetValue(key, out List<EventHandler> handlers))
            {
                foreach (var handler in handlers.AsReadOnly())
                {
                    handler.Invoke(sender, null);
                }
            }
        }

        public void Subscribe(string key, EventHandler action)
        {
            //var list = _eventDict.GetOrAdd(key, action);
            if (_eventDict.TryGetValue(key, out var handlers))
            {
                handlers.Add(action);
            }
            else
            {
                _eventDict.TryAdd(key, new List<EventHandler>() { action });
            }
        }

        public void Unsubscribe(string key, EventHandler action)
        {
            if (_eventDict.TryGetValue(key, out List<EventHandler> handlers))
            {
                handlers.Remove(action);
            }
        }

        public void Unsubscribe(string key)
        {
            _eventDict.TryRemove(key, out _);
        }
    }
    public class SimpleEventBus<T>
    {
        private readonly ConcurrentDictionary<string, List<EventHandler<T>>> _eventDict = new();

        private SimpleEventBus() { }
        public static SimpleEventBus<T> Instance { get; private set; } = new();

        public void Publish(string key, object sender, T message)
        {
            if (_eventDict.TryGetValue(key, out List<EventHandler<T>> handlers))
            {
                foreach (var handler in handlers.AsReadOnly())
                {
                    handler.Invoke(sender, message);
                }
            }
        }

        public void Subscribe(string key, EventHandler<T> action)
        {
            if (_eventDict.TryGetValue(key, out var handlers))
            {
                handlers.Add(action);
            }
            else
            {
                _eventDict.TryAdd(key, new List<EventHandler<T>>() { action });
            }
        }

        public void Unsubscribe(string key, EventHandler<T> action)
        {
            if (_eventDict.TryGetValue(key, out List<EventHandler<T>> handlers))
            {
                handlers.Remove(action);
            }
        }

        public void Unsubscribe(string key)
        {
            _eventDict.TryRemove(key, out _);
        }
    }
}
