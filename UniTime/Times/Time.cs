using Kratorg.Internal;
using Kratorg.Internal.Times;
using System;

namespace Kratorg
{
    public static class Time
    {
        static bool _initialized = false;
        static bool _disposed = false;
        
        static Updater  _timeKeeper;

        static Time()
        {
            Initialize();
        }

        static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _timeKeeper = new Updater();

            _initialized = true;
        }

        public static Clock Tick(Action action) => Tick((long _) => action());

        public static Clock Tick(Action<long> update)
        {
            if (update != null)
            {
                return null;
            }

            var clock = new Clock();
            clock.Subscribe(update);
            
            _timeKeeper.Subscribe(clock.Update);

            return clock;
        }

        //public static Timer Tick(Action action, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Timer(rate, mode);
        //    timer.ticked += (_) => action();
        //    timer.Start();

        //    return timer;
        //}

        //public static Timer Tick(Action<Timer> action, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Timer(rate, mode);
        //    timer.ticked += action;
        //    timer.Start();

        //    return timer;
        //}

        //public static Timer TickUntil(Action action, double duration, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Timer(rate, mode);
        //    timer.ended += (Timer t) => { action(); t.Dispose(); };
        //    timer.Start(duration);

        //    return timer;
        //}

        //public static Timer DoAfter(Action action, double duration, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Timer(rate, mode);
        //    timer.ended += (Timer t) => { action(); t.Dispose(); };
        //    timer.Start(duration);

        //    return timer;
        //}

        public static void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _timeKeeper.Dispose();
            _disposed = true;
        }
    }

    public enum ClockMode
    {
        Main,
        UnityPlayMode,
        UnityEditor
    }
}