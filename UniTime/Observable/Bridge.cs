using System;

namespace Kratorg.Internal
{
    public class Bridge<T> : IObservable<T>, IDisposable
    {
        bool _disposed;

        event Action<T> onNext;

        public Bridge(Action<T> action)
        {
            _disposed = false;
            action += onNext;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_disposed)
            {
                return null;
            }

            onNext += observer.OnNext;
            return new Unsubscriber(() => Unsubscribe(observer));
        }

        public void Unsubscribe(IObserver<T> observer)
        {
            if (_disposed || observer == null)
            {
                return;
            }

            observer.OnCompleted();

            onNext -= observer.OnNext;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            onNext = null;

            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}