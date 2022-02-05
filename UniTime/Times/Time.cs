using Kratorg.Internal;
using Kratorg.Internal.Times;
using System;
using UnityEditor;
using UnityEngine;
using Timer = Kratorg.Internal.Times.Timer;

namespace Kratorg
{
    [InitializeOnLoad]
    public static class Time
    {
        static bool _initialized = false;
        static bool _disposed = false;
        
        static Updater           _timeKeeper;
        static PlayModeUpdater   _playmodeTimeKeeper;
        static EditorUpdater     _editorTimeKeeper;

        public static IObservable<double> Updater         => _disposed ? null : _timeKeeper;
        public static IObservable<double> EditorUpdater   => _disposed ? null : _editorTimeKeeper;
        public static IObservable<double> PlayModeUpdater => _disposed ? null : _playmodeTimeKeeper;

        static Time()
        {
            Initialize();
        }

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _timeKeeper = new Updater();
            _editorTimeKeeper = new EditorUpdater();
            _playmodeTimeKeeper = new PlayModeUpdater();

            _initialized = true;
        }

        public static IObservable<double> ModeToUpdater(ClockMode mode) 
        {
            if (mode == ClockMode.Main)
            {
                return Updater;
            }
            else if (mode == ClockMode.UnityEditor)
            {
                return EditorUpdater;
            }

            return PlayModeUpdater;
        }

        public static Stopwatch Stopwatch(ClockMode mode) => Stopwatch(ModeToUpdater(mode), null);
        public static Stopwatch Stopwatch(Action<double> update) => Stopwatch(Updater, update);
        public static Stopwatch Stopwatch(ClockMode mode, Action<double> update) => Stopwatch(ModeToUpdater(mode), update);

        public static Stopwatch Stopwatch(IObservable<double> updater, Action<double> update)
        {
            Stopwatch stopwatch = new Stopwatch(Updater);

            if (update != null)
            {
                stopwatch.Subscribe(update);
            }

            return stopwatch;
        }

        //public static Timer Timer(double tickRate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    if (mode == ClockMode.Main)
        //    {
        //        return new Timer(Updater, tickRate);
        //    }
        //    else if (mode == ClockMode.UnityEditor)
        //    {
        //        return new Timer(PlayModeUpdater(), tickRate);
        //    }

        //    return new Timer(PlayModeUpdater(), tickRate);
        //}

        //public static Timer Timer(double duration, double tickRate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    Timer result;
        //    if (mode == ClockMode.Main)
        //    {
        //        result = new Timer(Updater(), tickRate);
        //    }
        //    else if (mode == ClockMode.UnityEditor)
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }
        //    else
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }

        //    result.ChangeDuration(duration);

        //    return result;
        //}

        //public static Timer Timer(Action<double> update, double tickRate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    Timer result;
        //    if (mode == ClockMode.Main)
        //    {
        //        result = new Timer(Updater(), tickRate);
        //    }
        //    else if (mode == ClockMode.UnityEditor)
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }
        //    else
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }

        //    result.Subscribe(update);

        //    return result;
        //}
        
        //public static Timer Timer(Action<Timer> end, double duration, double tickRate = 0.03D, ClockMode mode = ClockMode.Main)
        //{
        //    Timer result;
        //    if (mode == ClockMode.Main)
        //    {
        //        result = new Timer(Updater(), tickRate);
        //    }
        //    else if (mode == ClockMode.UnityEditor)
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }
        //    else
        //    {
        //        result = new Timer(PlayModeUpdater(), tickRate);
        //    }

        //    result.ended += end;
        //    result.ChangeDuration(duration);

        //    return result;
        //}

        //public static Timer Timer(IObservable<double> updater, double tickRate, double duration, Action<Timer> tick, Action<Timer> end)
        //{
        //    Timer timer = new Timer(Updater(), tickRate);
            
        //    result.ticked += tick;
        //    result.ended += end;
        //    result.ChangeDuration(duration);

        //    return result;
        //}

        public static Stopwatch Tick(Action action, ClockMode mode = ClockMode.Main)
        {
            var stopwatch = Stopwatch(mode);
            stopwatch.Subscribe((_) => action());
            stopwatch.Start();

            return stopwatch;
        }

        public static Stopwatch Tick(Action<double> action, ClockMode mode = ClockMode.Main)
        {
            var stopwatch = Stopwatch(mode);
            stopwatch.Subscribe(action);
            stopwatch.Start();

            return stopwatch;
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
            _editorTimeKeeper.Dispose();
            _playmodeTimeKeeper.Dispose();

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