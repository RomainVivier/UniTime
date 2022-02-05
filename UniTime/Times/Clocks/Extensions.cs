using System;

namespace Kratorg.Internal.Times
{
    public static class Extensions
    {
        public static void Tick(this Action action, ClockMode mode = ClockMode.Main) 
        {
            var stopwatch = Time.Stopwatch(mode);
            stopwatch.Subscribe((_) => action());
            stopwatch.Start();
        }

        public static void Tick(this Action<double> action, ClockMode mode = ClockMode.Main)
        {
            var stopwatch = Time.Stopwatch(mode);
            stopwatch.Subscribe(action);
            stopwatch.Start();
        }

        //public static void Tick(this Action action, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Time.Timer(rate, mode);
        //    timer.ticked += (_) => action();
        //    timer.Start();
        //}

        //public static void Tick(this Action<Timer> action, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Time.Timer(rate, mode);
        //    timer.ticked += action;
        //    timer.Start();
        //}

        //public static void TickUntil(this Action action, double duration, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Time.Timer(rate, mode);
        //    timer.ticked += (_) => action();
        //    timer.Start(duration);
        //}

        //public static void DoAfter(this Action action, double duration, double rate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    var timer = Time.Timer(rate, mode);
        //    timer.ended += (_) => action();
        //    timer.Start(duration);
        //}
    }
}