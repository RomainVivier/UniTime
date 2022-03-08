using System;
using System.Collections.Generic;

namespace Kratorg.Internal.Times
{
    public interface IClock
    {
        long StartingTick { get; }
        long LastTick { get; }
        TimeSpan Delta { get; }
        TimeSpan Past { get; }

        void Update(long value);
        IDisposable Decorate(Func<long, long> decorator);
        IDisposable Subscribe(Action<long> observer);
        IDisposable EndWhen(Predicate<IClock> predicate);
    }

    public sealed class Clock : IClock, IDisposable
    {
        bool                    _disposed   = false;
        int                     _isPaused   = -1;
        List<Func<long, long>>  _decorators = new List<Func<long, long>>();
        List<Predicate<IClock>> _ends       = new List<Predicate<IClock>>();

        /// <summary>
        /// Timestamp at which clock as been created
        /// </summary>
        public long StartingTick { get; } = DateTime.Now.Ticks;
        /// <summary>
        /// Timestamp of clock last Update
        /// </summary>
        public long LastTick     { get; }
        /// <summary>
        /// Time passed since <see cref="LastTick"/>last tick</see> and DateTime.Now.Tick
        /// </summary>
        public TimeSpan Delta => new TimeSpan(DateTime.Now.Ticks - LastTick);
        public TimeSpan Past     { get; private set; } = TimeSpan.Zero;

        public bool IsPaused
        {
            get => _isPaused >= 0;

            set 
            {
                if (IsPaused == value) return;

                if (value)
                {
                    _isPaused = _decorators.Count;
                    _decorators.Add(Paused);
                }
                else
                {
                    _decorators.RemoveAt(_isPaused);
                    _isPaused = -1;
                }
            }
        }
        public long Paused(long _) => 0L;

        event Action<long> ticked;
        
        public void Update(long value)
        {
            if (_disposed)
            {
                return;
            }

            int count = _decorators.Count;
            for (int i = 0; i < count; i++)
            {
                value = _decorators[i](value);
            }

            Past.Add(new TimeSpan(value));
            
            ticked?.Invoke(value);

            count = _ends.Count;
            for (int i = 0; i < count; i++)
            {
                if (_ends[i](this))
                {
                    Dispose();
                    break;
                }
            }
        }

        public IDisposable Decorate(Func<long, long> decorator)
        {
            int index = _decorators.IndexOf(decorator);
            if (_disposed || index >= 0 || decorator == null) return null;

            _decorators.Add(decorator);

            return new Unsubscriber(() => DecoratorUnsubscribe(decorator));
        }

        void DecoratorUnsubscribe(Func<long, long> decorator)
        {
            int index = _decorators.IndexOf(decorator);
            if (_disposed || index < 0 || decorator == null)
            {
                return;
            }

            _decorators.RemoveAt(index);
        }

        public IDisposable Subscribe(Action<long> observer)
        {
            if (_disposed)
            {
                return null;
            }

            ticked += observer;

            return new Unsubscriber(() => TickUnsubscribe(observer));
        }

        void TickUnsubscribe(Action<long> observer)
        {
            if (_disposed || observer == null)
            {
                return;
            }

            ticked -= observer;
        }

        public IDisposable EndWhen(Predicate<IClock> predicate)
        {
            int index = _ends.IndexOf(predicate);
            if (_disposed || index >= 0 || predicate == null)
            {
                return null;
            }

            _ends.Add(predicate);

            return new Unsubscriber(() => EndUnsubscribe(predicate));
        }

        void EndUnsubscribe(Predicate<IClock> predicate)
        {
            int index = _ends.IndexOf(predicate);
            if (_disposed || index < 0 || predicate == null)
            {
                return;
            }

            _ends.RemoveAt(index);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            ticked = null;
            _decorators.Clear();
            _ends.Clear();

            GC.SuppressFinalize(this);
        }
    }
};