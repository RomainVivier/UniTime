using System;

namespace Kratorg.Internal.Times
{
    public static class Extensions
    {
        public static Clock Tick(this Action action) => Time.Tick(action);
        public static Clock Tick(this Action<long> action)=> Time.Tick(action);

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