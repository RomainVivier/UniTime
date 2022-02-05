using System;

namespace Kratorg.Internal.Times
{
    public class Timer : Stopwatch
    {
        protected double _tickRate   = 0.1D;
        protected double _nextTick   = 0D;
        protected double _duration   = 0D;

        public event Action<Timer>  ticked;
        public event Action<Timer>  ended;

        public double PastNormalized => Math.Min(Math.Max((Seconds / _duration), 1D), 0D);

        public double Left => _duration - Seconds;
        public double LeftNormalized => 1D - PastNormalized;

        public bool IsOver => _duration > 0D && Seconds >= _duration;
         
        public Timer(IObservable<double> updater, double tickRate = 0.1D) : base(updater) => _tickRate = tickRate;

        //public Timer(IObservable<double> updater, double tickRate = 0.1D) : base(updater) => _tickRate = tickRate;

        //public Timer(IObservable<double> updater, double tickRate = 0.1D) : base(updater) => _tickRate = tickRate;

        public Timer(IObservable<double> updater, double tickRate, double duration, Action<Timer> onTick, Action<Timer> onEnd) : base(updater) => _tickRate = tickRate;

        public void Start(double duration = 0D)
        {
            isPaused.Value = true;

            if (duration > 0D)
            {
                ChangeDuration(duration);
            }

            Start();
        }

        public void ChangeDuration(double duration)
        {
            _duration = duration;

            if (isPaused == false && IsOver)
            {
                OnCompleted();
            }
        }

        public override void Update(double delta)
        {
            base.Update(delta);

            _nextTick -= delta;
                
            if (_tickRate <= 0D || _nextTick <= 0D)
            {
                _nextTick = _tickRate;
                ticked?.Invoke(this);
            }

            if (IsOver)
            {
                OnCompleted();
            }
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            ended?.Invoke(this);
        }

        public override void OnDispose() 
        {
            ticked = null;
            ended = null;

            base.OnDispose();
        }
    }
}