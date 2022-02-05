using System;

namespace Kratorg.Internal.Times
{
    public class Clock : IObservable<double>, IDisposable
    {
        protected bool          _disposed   = false;
        protected TimeSpan      _span       = TimeSpan.Zero;
        
        public double           modifier    = 1f;
        public Symbol<bool>     isPaused    = new Symbol<bool>(true);

        event Action<double>    ticked;
        event Action            disposed;

        public DateTime First { get; protected set; } = DateTime.Now;
        public TimeSpan Past => _span;
        public double   Hours => _span.TotalHours;
        public double   Minutes => _span.TotalMinutes;
        public double   Seconds => _span.TotalSeconds;
        public double   Milliseconds => _span.TotalMilliseconds;
        public long     Ticks   => _span.Ticks;

        public virtual void Update(double value)
        {
            if (isPaused.Value)
            {
                return;
            }

            value   *= modifier;
            _span.Add(new TimeSpan((long)(value * TimeSpan.TicksPerSecond)));

            Raise(value);
        }

        protected virtual void Raise(double delta)
        {
            ticked?.Invoke(delta);
        }

        public IDisposable Subscribe(IObserver<double> observer)
        {
            if (_disposed) return null;

            ticked += observer.OnNext;
            
            Action disposal = () => Unsubscribe(observer);
            disposed += disposal;

            return new Unsubscriber(disposal);
        }

        void Unsubscribe(IObserver<double> observer)
        {
            if (_disposed || observer == null) return;

            observer.OnCompleted();

            ticked -= observer.OnNext;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            
            _disposed = true;

            OnDispose();

            GC.SuppressFinalize(this);
        }

        public virtual void OnDispose() 
        {
            disposed();

            disposed = null;
            ticked = null;
            isPaused.Clear();
        }
    }
}