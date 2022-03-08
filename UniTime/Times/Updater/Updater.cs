using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kratorg.Internal.Times
{
    internal sealed class Updater : IDisposable
    {
        bool _disposed  = false;
        long _lastFrame = DateTime.Now.Ticks;
        List<Action<long>> _subscribers = new List<Action<long>>();
        
        public Updater()
        {
            UpdateLoop();
        }

        public IDisposable Subscribe(Action<long> action)
        {
            int index = _subscribers.IndexOf(action);
            if (_disposed || index >= 0 || action == null)
            {
                return null;
            }

            _subscribers.Add(action);

            return new Unsubscriber(() => EndUnsubscribe(action));
        }

        void EndUnsubscribe(Action<long> action)
        {
            int index = _subscribers.IndexOf(action);
            if (_disposed || index < 0 || action == null)
            {
                return;
            }

            _subscribers.RemoveAt(index);
        }

        async void UpdateLoop()
        {
            while (_disposed == false)
            {
                await Task.Yield();

                lock (this)
                {
                    long delta = DateTime.Now.Ticks - _lastFrame;
                    
                    int count = _subscribers.Count;
                    for (int i = 0; i < count; i++)
                    {
                        _subscribers[i](delta);
                    }

                    _lastFrame = DateTime.Now.Ticks;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _subscribers.Clear();
            
            GC.SuppressFinalize(this);
        }
    }
}