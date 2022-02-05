using System;
using System.Threading.Tasks;

namespace Kratorg.Internal.Times
{
    internal sealed class Updater : Clock
    {
        DateTime _lastFrame;
        
        public Updater()
        {
            _lastFrame = DateTime.Now;
            UpdateLoop();
        }

        async void UpdateLoop()
        {
            while (_disposed == false)
            {
                await Task.Yield();

                lock (this)
                {
                    TimeSpan past = DateTime.Now - _lastFrame;
                    Update(past.TotalSeconds);
                    _lastFrame = DateTime.Now;
                }
            }
        }
    }
}